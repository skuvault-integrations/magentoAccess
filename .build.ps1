<#
.Synopsis
	Build script (https://github.com/nightroman/Invoke-Build)

.Description
	How to use this script and build the module:

	Get the utility script Invoke-Build.ps1:
	https://github.com/nightroman/Invoke-Build

	Copy it to the path. Set location to this directory. Build:
	PS> Invoke-Build Build

	This command builds the module and installs it to the $ModuleRoot which is
	the working location of the module. The build fails if the module is
	currently in use. Ensure it is not and then repeat.

	The build task Help fails if the help builder Helps is not installed.
	Ignore this or better get and use the script (it is really easy):
	https://github.com/nightroman/Helps
#>

param
(
	$Configuration = 'Release',
	$logfile = $null
)

$project_name = "MagentoAccess"
$project_short_name = "Magento"

# Folder structure:
# \build - Contains all code during the build process
# \build\artifacts - Contains all files during intermidiate bulid process
# \build\output - Contains the final result of the build process
# \release - Contains final release files for upload
# \release\archive - Contains files archived from the previous builds
# \src - Contains all source code
$build_dir = "$BuildRoot\build"
$log_dir = "$BuildRoot\log"
$build_artifacts_dir = "$build_dir\artifacts"
$build_output_dir = "$build_dir\output"
$release_dir = "$BuildRoot\release"
$archive_dir = "$release_dir\archive"

$src_dir = "$BuildRoot\src"
$solution_file = "$src_dir\MagentoAccess.sln"
	
# Use MSBuild.
use Framework\v4.0.30319 MSBuild

task Clean { 
	exec { MSBuild "$solution_file" /t:Clean /p:Configuration=$configuration /v:quiet } 
	Remove-Item -force -recurse $build_dir -ErrorAction SilentlyContinue | Out-Null
}

task Init Clean, { 
    New-Item $build_dir -itemType directory | Out-Null
    New-Item $build_artifacts_dir -itemType directory | Out-Null
    New-Item $build_output_dir -itemType directory | Out-Null
}

task Build {
	exec { MSBuild "$solution_file" /t:Build /p:Configuration=$configuration /v:minimal /p:OutDir="$build_artifacts_dir\" }
}

task Package  {
	New-Item $build_output_dir\MagentoAccess\lib\net45 -itemType directory -force | Out-Null
	Copy-Item $build_artifacts_dir\MagentoAccess.??? $build_output_dir\MagentoAccess\lib\net45 -PassThru |% { Write-Host "Copied " $_.FullName }
}

# Set $script:Version = assembly version
task Version {
	assert (( Get-Item $build_artifacts_dir\MagentoAccess.dll ).VersionInfo.FileVersion -match '^(\d+\.\d+\.\d+)')
	$script:Version = $matches[1]
}

task Archive {
	New-Item $release_dir -ItemType directory -Force | Out-Null
	New-Item $archive_dir -ItemType directory -Force | Out-Null
	Move-Item -Path $release_dir\*.* -Destination $archive_dir
}

task Zip Version, {
	$release_zip_file = "$release_dir\$project_name.$Version.zip"
	$7z = Get-ChildItem -recurse $src_dir\packages -include 7za.exe | Sort-Object LastWriteTime -descending | Select-Object -First 1
	
	Write-Host "Zipping release to: " $release_zip_file
	
	exec { & $7z a $release_zip_file $build_output_dir\$project_name\lib\net45\* -mx9 }
}

task NuGet Package, Version, {

	Write-Host ================= Preparing MagentoAccess Nuget package =================
	$text = "$project_short_name webservices API wrapper."
	# nuspec
	Set-Content $build_output_dir\MagentoAccess\MagentoAccess.nuspec @"
<?xml version="1.0"?>
<package>
	<metadata>
		<id>MagentoAccess</id>
		<version>$Version-rc3</version>
		<authors>Slav Ivanyuk</authors>
		<owners>Slav Ivanyuk</owners>
		<projectUrl>https://github.com/slav/MagentoAccess</projectUrl>
		<licenseUrl>https://raw.github.com/slav/MagentoAccess/master/License.txt</licenseUrl>
		<requireLicenseAcceptance>false</requireLicenseAcceptance>
		<copyright>Copyright (C) Agile Harbor, LLC 2014</copyright>
		<summary>$text</summary>
		<description>$text</description>
		<tags>$project_short_name</tags>
		<dependencies> 
			<group targetFramework="net45">
				<dependency id="Netco" version="1.3.1" />
				<dependency id="CuttingEdge.Conditions" version="1.2.0.0" />
				<dependency id="DotNetOpenAuth.OAuth.Consumer" version="4.3.4.13329" />
			</group>
		</dependencies>
	</metadata>
</package>
"@
	# pack
	$nuget = "$($src_dir)\.nuget\NuGet"
	
	exec { & $nuget pack $build_output_dir\$project_name\$project_name.nuspec -Output $build_dir }
	
	$push_project = Read-Host "Push $($project_name) " $Version " to NuGet? (Y/N)"
	Write-Host $push_project
	if( $push_project -eq "y" -or $push_project -eq "Y" )	{
		Get-ChildItem $build_dir\*.nupkg |% { exec { & $nuget push  $_.FullName }}
	}
}

task . Init, Build, Package, Zip, NuGet