﻿/*
Copyright (C) 2016 by Eric Bataille <e.c.p.bataille@gmail.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace ThoNohT.NohBoard
{
    /// <summary>
    /// Class that contains the current version of NohBoard.
    /// </summary>
    public static class Version
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        public const string Get = "v1.3.0.6~53eb8a6*";

        /// <summary>
        /// Gets the major version.
        /// </summary>
        public static int Major = int.Parse(Get.Substring(1).Split('.')[0]);

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        public static int Minor = int.Parse(Get.Substring(1).Split('.')[1]);

        /// <summary>
        /// Gets the patch version.
        /// </summary>
        public static int Patch = int.Parse(Get.Substring(1).TrimEnd('*').Split('.')[2]);
    }
}