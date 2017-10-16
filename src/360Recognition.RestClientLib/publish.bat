@echo off

echo ***REMEMBER TO BUILD IN RELEASE BEFORE RUNNING THIS COMMAND***

echo creating package
call pack.cmd

echo publishing package to feed
call push.cmd

DEL *.nupkg