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
// Ian MaceLean (ian@maclean.ms)
// Gert Driesen (gert.driesen@ardatis.com)

using System;
using System.Reflection;

// DO NOT mark NDoc.Documenter.NAnt CLS compliant
// because NDoc is not CLS-compliant
[assembly: CLSCompliant(false)]

// This will not compile with Visual Studio.  If you want to build a signed
// executable use the NAnt build file.  To build under Visual Studio just
// exclude this file from the build.
[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile(@"..\NAnt.key")]
[assembly: AssemblyKeyName("")]
