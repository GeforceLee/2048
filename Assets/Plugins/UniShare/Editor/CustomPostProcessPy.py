#!/usr/bin/env python 
import plistlib
import sys, os.path
from mod_pbxproj import XcodeProject

def importHeaders():
    return '''
#import "../Libraries/UniSocialNativeToolkit.h"
#import "../Libraries/WXApi.h"
'''

def openURL():
	return '''
	
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
{
	return [WXApi handleOpenURL:url delegate:[UniSocialNativeToolkit sharedManager]];
}

'''
print '-----------------------------'
install_path = sys.argv[1] 
unityVersion = sys.argv[2]
schemes = sys.argv[3] 
print install_path
#add url schemes to Info.plist
info_plist_path = os.path.join(install_path, 'Info.plist') 
pl = plistlib.readPlist(info_plist_path)
new_settings = {"CFBundleURLSchemes": [schemes]}
if "CFBundleURLTypes" in pl:
	pl["CFBundleURLTypes"].extend(new_settings)
else:
	pl["CFBundleURLTypes"] = [new_settings]
plistlib.writePlist(pl, info_plist_path)

#modify appcontroller.mm(UnityAppcontroller.mm for unity 4.2)
isBelow4_2 = unityVersion<'4.2'
if isBelow4_2:
	mmPath = os.path.join(install_path, 'Classes/AppController.mm')
else:
	mmPath = os.path.join(install_path, 'Classes/UnityAppController.mm')
print mmPath
appcontroller = open(mmPath, 'r')
content = appcontroller.read()
print content
appcontroller.close()
pos = -1
pos = content.find('openURL')
if pos<0:
	if isBelow4_2:
		pos = content.find('@implementation AppController')
		if pos!=-1:
			pos += 29
			content = content[:pos] + openURL() + content[pos:]
	else:
		pos = content.rfind('@end')
		if pos!=-1:
			content = content[:pos] + openURL() + content[pos:]
	

pos = content.find('UniSocialNativeToolkit.h')
if pos<0:
	content = importHeaders() + content
appcontroller = open(mmPath, 'w')
appcontroller.write(content)
appcontroller.close()
print content
#remove -all_load flag which conflict with wechat
projectFilePath = os.path.join(install_path, 'Unity-iPhone.xcodeproj/project.pbxproj')
project = XcodeProject.Load(projectFilePath)
project.remove_other_ldflags('-all_load')
#project.add_other_ldflags('test')

print project.modified
if project.modified:
    project.backup()
    project.saveFormat3_2()