'use strict'
var gulp = require('gulp');
var gulpsync = require('gulp-sync')(gulp);
var execSync = require('child_process').execSync;
var filesExist = require('files-exist');
var rimraf = require('rimraf');
var upath = require('upath');
var fs = require('fs');

function isWindows(){
    return /^win/.test(process.platform);
}

function getConvertedPath(pPath){
    if(isWindows()){
        return pPath.replace(/\//g,"\\");
    }
    else{
        return upath.toUnix(pPath);
    }
};

var tasks = {
    buildAll: 'build-all',
    runTests: 'run-tests',
    buildPro: 'build-pro',
    createTempBuildProject: 'create-temp-build-project',
    copyProFilesToTempProject: 'copy-pro-files-to-temp-project',
    removeTempBuildProject: 'remove-temp-build-project',
    createProUnityPackage: 'create-pro-unity-package',
};

var paths = {
    dist: getConvertedPath('dist\\'),
    tempBuildFolder: getConvertedPath('TempBuildProject\\')
}


gulp.task('default', [tasks.buildAll]);
gulp.task(tasks.buildAll, gulpsync.sync([tasks.runTests, tasks.buildPro]));
gulp.task(tasks.buildPro, gulpsync.sync([tasks.createTempBuildProject, tasks.copyProFilesToTempProject, tasks.createProUnityPackage, tasks.removeTempBuildProject]));


gulp.task(tasks.runTests, function() {
	runUnityJob('-quit -batchmode -executeMethod UnityTest.Batch.RunUnitTests -resultFilePath=unitTestResults.xml');
});


gulp.task(tasks.removeTempBuildProject, function() {
    rimraf.sync('./' + paths.tempBuildFolder);
    console.log(paths.tempBuildFolder + ' was removed!');
});

gulp.task(tasks.createTempBuildProject, function() {
    return gulp.src('ProjectSettings/**/*.*') 
           .pipe(gulp.dest(paths.tempBuildFolder + getConvertedPath('ProjectSettings/')));
});

gulp.task(tasks.copyProFilesToTempProject, function() {
    return gulp.src('Assets/SmartLocalization/**/*.*') 
           .pipe(gulp.dest(paths.tempBuildFolder + getConvertedPath('Assets/SmartLocalization/')));
});


gulp.task(tasks.createProUnityPackage, function() {
    var d = new Date();
    var unityPackageName = 'SmartLocalization_PRO_';
	unityPackageName += d.getFullYear() + '-' + d.getMonth()+'-' + d.getDate() + '_' + (d.getHours() + 1) + '-' + d.getMinutes() + '-' + d.getSeconds();
    unityPackageName += '.unitypackage';
    runUnityJob('-quit -batchmode -projectPath \"' + __dirname + '\\' + paths.tempBuildFolder + '\" -exportPackage \"Assets\\SmartLocalization\" \"' + unityPackageName + '\"');
    return gulp.src('TempBuildProject/*.unitypackage') 
           .pipe(gulp.dest(paths.dist));
});

function getDefaultUnityPath(){
    if(isWindows() === true){
     return 'C:\\Program Files\\Unity\\Editor\\Unity.exe';
    }
    return '/Applications/Unity/Unity.app/Contents/MacOS/Unity';
}


function runUnityJob(pArguments){
    var unityPath = process.env.npm_config_unityPath;
    if(unityPath === null || unityPath === undefined){
        console.log('a valid unity path was not provided. provide with npm --unityPath=<YOUR_PATH> run gulp <YOURTASK>');
        unityPath = getDefaultUnityPath();
        console.log('Trying default unity path:' + unityPath);
    }
    if(pArguments === null || pArguments === undefined){
        throw 'invalid arguments in unity job';
    }

    executeCommand('\"' + unityPath + '\" ' + pArguments);
}

function executeCommand(pFullCommand){
    var commandToExecute = getConvertedPath(pFullCommand);;
    console.log('Executing command: ' + commandToExecute);
    try {
        var output = execSync(commandToExecute, {encoding:'utf-8'});
        console.log(String(output));
    }
    catch(error) {
        throw error.stdout.toString();
    }
}