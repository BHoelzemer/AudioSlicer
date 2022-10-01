sign third part library https://stackoverflow.com/questions/331520/how-to-fix-referenced-assembly-does-not-have-a-strong-name-error
ildasm /all /out=thirdPartyLib.il thirdPartyLib.dll //in debug
ilasm /dll /key=myKey.snk thirdPartyLib.il