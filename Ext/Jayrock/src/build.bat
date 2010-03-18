@echo off

REM Batch file for building Jayrock using NAnt
REM
REM Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
REM Written by Atif Aziz (atif.aziz@skybow.com)
REM Copyright (c) 2005 Atif Aziz. All rights reserved.
REM
REM License, Terms and Conditions
REM
REM This library is free software; you can redistribute it and/or modify it under
REM the terms of the GNU Lesser General Public License as published by the Free
REM Software Foundation; either version 2.1 of the License, or (at your option)
REM any later version.
REM
REM This library is distributed in the hope that it will be useful, but WITHOUT
REM ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
REM FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
REM details.
REM
REM You should have received a copy of the GNU Lesser General Public License
REM along with this library; if not, write to the Free Software Foundation, Inc.,
REM 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

@pushd %~dp0
..\tools\nant-0.85\NAnt -t:net-1.1 %*
@popd