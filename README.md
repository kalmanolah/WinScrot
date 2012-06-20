WinScrot (.NET edition), by Kalman Olah

Download: https://github.com/downloads/kalmanolah/WinScrot/WinScrot.exe

Usage: WinScrot [-d DELAY] [-c] [-q QUALITY] [-t THUMBSIZE] [FILENAME.EXT]

    -d INT       Delay before the screenshot is taken, in seconds. Default: 0. Min: 0. Optional.
    -c           Displays a countdown when used with delay. Default: False. Optional.
    -q INT       Quality of the screenshot, in percentages. Default: 75. Min: 1. Max: 100. Optional.
                 NOTE: Differs depending on image format.
    -t INT       Resolution of the screenshot, in percentages of full desktop size. Default: 100. Min: 1. Max: 100. Optional.
    file.ext     Filename of the screenshot, relative or absolute. Allowed extensions: png, jpg, jpeg, gif, bmp. Optional.
                 NOTE: DateTime substitutions can be used in the filename in the "$dt.<example>$" format.
                 e.g.: 'WinScrot-$dt.dd.MM.yyyy--HH.mm.ss$.png' -> 'WinScrot-01.07.2012--17.48.53.png'
-

    WinScrot - A command-line screenshotting utility
    Copyright (C) 2012  Kalman Olah

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.