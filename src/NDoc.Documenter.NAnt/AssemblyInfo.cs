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

// Ian MaceLean (ian@maclean.ms)

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// DO NOT mark NDoc.Documenter.NAnt CLS compliant
// because NDoc is not CLS-compliant
[assembly: CLSCompliant(false)]

// Make NDoc.Documenter.NAnt as NOT visible to COM
[assembly: ComVisible(false)]

[assembly: AssemblyTitle("NAnt Documenter")]
[assembly: AssemblyDescription("NDoc Documenter for NAnt")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("http://nant.sf.net")]
[assembly: AssemblyProduct("NAnt")]
[assembly: AssemblyCopyright("Copyright (C) 2001-2002 Gerry Shaw")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("0.8.2.*")]

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
