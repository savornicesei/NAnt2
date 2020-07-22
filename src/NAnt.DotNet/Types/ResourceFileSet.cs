// NAnt - A .NET build tool
// Copyright (C) 2001-2002 Gerry Shaw
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Gerry Shaw (gerry_shaw@yahoo.com)
// Ian MacLean ( ian_maclean@another.com )

using System;
using System.Globalization;
using System.IO;
using System.Text;

using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;

namespace NAnt.DotNet.Types {
    /// <summary>
    /// Specialized <see cref="FileSet" /> class for managing resource files.
    /// </summary>
    [ElementName("resourcefileset")]
    public class ResourceFileSet : FileSet, ICloneable {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceFileSet" /> class.
        /// </summary>
        public ResourceFileSet() : base() {
        }

        /// <summary>
        /// copy constructor for FileSet. Required in order to 
        /// assign references of FileSet type where 
        /// ResourceFileSet are used
        /// </summary>
        /// <param name="fs"></param>
        public ResourceFileSet(FileSet fs) : base(fs) {
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Indicates the prefix to prepend to the actual resource. 
        /// This is usually the default namespace of the assembly.
        /// </summary>
        [TaskAttribute("prefix")]
        public string Prefix {
            get { return _prefix; }
            set { _prefix = StringUtils.ConvertEmptyToNull(value); }
        }

        /// <summary>
        /// Indicates whether prefixes should be dynamically generated by taking 
        /// the path of the resource relative to the base directory and appending it 
        /// to the specified prefix. The default is <see langword="false" />.
        /// </summary>
        [BooleanValidator()]
        [TaskAttribute("dynamicprefix")]
        public bool DynamicPrefix { 
            get { return _dynamicprefix; }
            set { _dynamicprefix = value; }
        }

        /// <summary>
        /// Gets a <see cref="FileSet" /> containing all matching resx files.
        /// </summary>
        /// <value>
        /// A <see cref="FileSet" /> containing all matching resx files.
        /// </value>
        public FileSet ResxFiles {
            get {
                ResourceFileSet retFileSet = (ResourceFileSet) this.Clone();
                retFileSet.Includes.Clear();
                retFileSet.Excludes.Clear();
                retFileSet.AsIs.Clear();
                retFileSet.FailOnEmpty = false;
                foreach (string file in FileNames) {
                    if (Path.GetExtension(file).ToLower(CultureInfo.InvariantCulture) == ".resx" ) {
                        retFileSet.Includes.Add(file);
                    }
                }
                retFileSet.Scan();
                return retFileSet;
            }
        }

        /// <summary>
        /// Gets a <see cref="FileSet" /> containing all matching non-resx 
        /// files.
        /// </summary>
        /// <value>
        /// A <see cref="FileSet" /> containing all matching non-resx files.
        /// </value>
        public FileSet NonResxFiles {
            get {
                ResourceFileSet retFileSet = (ResourceFileSet) this.Clone();
                retFileSet.Includes.Clear();
                retFileSet.Excludes.Clear();
                retFileSet.AsIs.Clear();
                retFileSet.FailOnEmpty = false;
                foreach (string file in FileNames) {
                    if (Path.GetExtension(file).ToLower(CultureInfo.InvariantCulture) != ".resx" ) {
                        retFileSet.Includes.Add(file);
                    }
                }
                retFileSet.Scan();
                return retFileSet;
            }
        }

        #endregion Public Instance Properties

        #region Implementation of ICloneable

        /// <summary>
        /// Creates a shallow copy of the <see cref="ResourceFileSet" />.
        /// </summary>
        /// <returns>
        /// A shallow copy of the <see cref="ResourceFileSet" />.
        /// </returns>
        public override object Clone() {
            ResourceFileSet clone = new ResourceFileSet();
            base.CopyTo(clone);
            clone._dynamicprefix = _dynamicprefix;
            clone._prefix = _prefix;
            return clone;
        }

        #endregion Implementation of ICloneable

        #region Public Instance Methods

        /// <summary>
        /// Gets the manifest resource name for the specified resource file.
        /// </summary>
        /// <param name="resourceFile">The physical path of the resource file.</param>
        /// <returns>
        /// The manifest resource name to be sent to the compiler.
        /// </returns>
        public string GetManifestResourceName(string resourceFile) {
            return GetManifestResourceName(resourceFile, resourceFile);
        }

        /// <summary>
        /// Gets the manifest resource name for the file using both its physical
        /// and logical path.
        /// </summary>
        /// <param name="physicalPath">The physical path of the resource file.</param>
        /// <param name="logicalPath">The logical location of the resource file.</param>
        /// <returns>
        /// The manifest resource name to be sent to the compiler.
        /// </returns>
        /// <remarks>
        /// We use the relative path of the logical path, but the filename and
        /// and the extension of the physical path to match VS.NET
        /// </remarks>
        public string GetManifestResourceName(string physicalPath, string logicalPath) {
            StringBuilder prefix = new StringBuilder(Prefix);

            if (DynamicPrefix) {
                string basedir = BaseDirectory.FullName;
                // ensure basedir ends with directory separator character
                if (!basedir.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))) {
                    basedir += Path.DirectorySeparatorChar;
                }
                // ensure filedir ends with directory separator character
                string filedir = Path.GetDirectoryName(logicalPath);
                if (!filedir.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))) {
                    filedir += Path.DirectorySeparatorChar;
                }
                string filePathRelativeToBaseDir = string.Empty;
                if (filedir != basedir && filedir.StartsWith(basedir)) {
                    filePathRelativeToBaseDir = filedir.Substring(basedir.Length);
                }
                string relativePrefix = filePathRelativeToBaseDir.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.');
                if (prefix.Length > 0) {
                    prefix.Append(".");
                }
                prefix.Append(relativePrefix);
            }

            StringBuilder manifestResourceName = new StringBuilder();

            // perform the following character operations :
            // 1) if a part of the prefix of a manifest resource name starts with
            //    a digit, prefix it with an underscore
            //    eg. Namespace.16x16.1image.if -> Namespace._16x16.1image.gif
            // 2) characters in the prefix that are neither letter nor digit
            //    must be replaced with underscores

            string[] parts = prefix.ToString().Split('.');
            for (int i = 0; i < parts.Length; i++) {
                string currentPart = parts[i];
                if (currentPart.Length == 0) {
                    // skip empty parts
                    continue;
                }
                for (int charIndex = 0; charIndex < currentPart.Length; charIndex++) {
                    char currentChar = currentPart[charIndex];
                    if (charIndex == 0 && char.IsDigit(currentChar)) {
                        manifestResourceName.Append('_');
                        manifestResourceName.Append(currentChar);
                    } else if (!char.IsLetterOrDigit(currentChar)) {
                        manifestResourceName.Append('_');
                    } else {
                        manifestResourceName.Append(currentChar);
                    }
                }
                manifestResourceName.Append('.');
            }

            // add filename to manifest resource name as is, VS.NET apparently
            // ignore the logical filename and uses the name of the physical file
            manifestResourceName.Append(Path.GetFileName(physicalPath));
            // return manifest resource name
            return manifestResourceName.ToString();
        }

        #endregion Public Instance Methods

        #region Private Instance Fields

        private string _prefix;
        private bool _dynamicprefix;

        #endregion Private Instance Fields
    }
}
