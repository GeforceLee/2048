//
//  UniSocialNativeToolkit.h
//  UniSocial
//
//  Created by Ren Chonghui on 11/20/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <QuartzCore/QuartzCore.h>
#import "WXApi.h"
typedef enum{
    PLATFORM_SINAWEIBO,
    PLATFORM_TENCENTWEIBO,
    PLATFORM_RENREN,
    PLATFORM_KAIXIN,
    PLATFORM_DOUBAN,
    PLATFORM_WEIXIN,
    PLATFORM_FACEBOOK,
    PLATFORM_TWITTER,
	PLATFORM_LINKEDIN,
	PLATFORM_FOURSQUARE,
	PLATFORM_GOOGLEPLUS,
	PLATFORM_QZONE
}PlatformType;

typedef enum{
    WXMESSAGE_TYPE_TEXT,
    WXMESSAGE_TYPE_IMG,
    WXMESSAGE_TYPE_MUSIC,
    WXMESSAGE_TYPE_VIDEO,
    WXMESSAGE_TYPE_WEBPAGE
}WechatMessageType;
//enum PlatformType
//{
//    SINA_WEIBO,
//    TENCENT_WEIBO,
//    RENREN
//};
@interface UniSocialNativeToolkit : NSObject<WXApiDelegate>

@property (nonatomic, retain) NSString *animationType;
@property (nonatomic, assign) CFTimeInterval animationDuration;
@property (nonatomic, retain) CAMediaTimingFunction *animationTimingFunction;
@property (nonatomic, assign) PlatformType platformType;
@property (nonatomic, retain) UINavigationController *navigationController;

+ (UniSocialNativeToolkit *)sharedManager;

- (void)loadUrl:(NSString *)aUrl platformType:(PlatformType)aPlatformType;
- (void)logout:(PlatformType)aPlatformType;
- (void)onApplicationOpenURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication;

- (void)shareToWechat:(WechatMessageType)aMessageType titleString:(NSString *)aTitleString content:(NSString *)aContentString url:(NSString *)aUrlString scene:(int)aScene;
- (void)shareToWechatWithTitle:(NSString *)aTitleString content:(NSString *)aContentString url:(NSString *)aUrlString scene:(int)aScene;
- (void)shareToWechatWithImagePath:(NSString *)aImagePath scene:(int)aScene;
@end

void _LoadUrl(const char *url, int platformType);
void _Logout(int platformType);

// for wechat
void _registerWeixinApi(const char *urlSchemes);
bool _isWechatAvailable();
void _shareToWechat(int msgType, const char *titleString, const char *content, const char *thumbImagePath, const char *url, int scene);
void _shareToWechatWithText(const char *titleString, const char *content, const char *thumbImagePath, const char *url, int scene);
void _shareToWechatWithImage(const char *imagePath, int scene);
