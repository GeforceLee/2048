
//
//  UniSocialNativeToolkit.m
//  UniSocial
//
//  Created by Ren Chonghui on 11/20/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "OAuthViewController.h"
#import "UniSocialNativeToolkit.h"
#import <Foundation/NSURLCache.h>
// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

void UnityPause( bool pause );
void UnitySendMessage( const char * obj, const char * method, const char * msg );
UIViewController *UnityGetGLViewController();

@implementation UniSocialNativeToolkit
@synthesize animationType = _animationType;
@synthesize animationDuration = _animationDuration;
@synthesize animationTimingFunction = _animationTimingFunction;
//@synthesize platformType = _platformType;
+ (UniSocialNativeToolkit *)sharedManager
{
    static UniSocialNativeToolkit *sharedManager = nil;
    
    if( !sharedManager )
        sharedManager = [[UniSocialNativeToolkit alloc] init];
    
    return sharedManager;
}

- (id)init
{
    if( ( self = [super init] ) )
    {
        // Default to fade animation and reasonable duration
        self.animationType = kCATransitionFade;
        self.animationDuration = 0.3;
        self.animationTimingFunction = [CAMediaTimingFunction functionWithName:kCAMediaTimingFunctionEaseIn];   

    }
    return self;
}

- (void)dealloc
{
    [super dealloc];
    [self.animationType release];
    self.navigationController = nil;
}
#pragma mark - Tool Func
- (UIImage *)scaledImage:(UIImage *)aImage ofSize:(CGSize)aSize
{
    if (aImage == nil)
    {
        return nil;
    }
    
    CGImageRef imgRef = aImage.CGImage;
    CGFloat width = CGImageGetWidth(imgRef);
    CGFloat height = CGImageGetHeight(imgRef);
    CGRect bounds = CGRectMake(0.0f, 0.0f, width, height);
    
    //if already at the minimum resolution, return the orginal image, otherwise scale
    if (width <= aSize.width && height <= aSize.height) {
        return aImage;
    }
    else {
        CGFloat ratio = width * aSize.height / (height * aSize.width);
        
        if (ratio < 1.0f) {
            bounds.size.width = aSize.width;
            bounds.size.height = aSize.height / ratio;
        } else {
            bounds.size.height = aSize.height;
            bounds.size.width = aSize.width * ratio;
        }
    }
    
    UIGraphicsBeginImageContextWithOptions(bounds.size, YES, 0.0f);
    [aImage drawInRect:bounds];
    UIImage *scaledImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    return scaledImage;
}

#pragma mark - 功能函数
- (void)loadUrl:(NSString *)aUrl platformType:(PlatformType)aPlatformType
{
    //OAuthViewController *oauthViewController = [[OAuthViewController alloc] init];
    OAuthViewController *oauthViewController = [[OAuthViewController alloc] initWithUrl:aUrl platform:aPlatformType];
    
    self.navigationController = [[UINavigationController alloc] initWithRootViewController:oauthViewController];
    [oauthViewController release];
    self.navigationController.modalPresentationStyle = UIModalPresentationFormSheet;
    self.navigationController.navigationBarHidden = NO;
    
    UIWindow *window = [UIApplication sharedApplication].keyWindow;
    UIViewController *rootViewController = window.rootViewController;
    [rootViewController dismissModalViewControllerAnimated:NO];
    [rootViewController presentModalViewController:self.navigationController animated:YES];

    
}

- (void)logout:(PlatformType)aPlatformType
{
    [[NSURLCache sharedURLCache] removeAllCachedResponses];
    for (NSHTTPCookie *cookie in [[NSHTTPCookieStorage sharedHTTPCookieStorage] cookies]) {
        [[NSHTTPCookieStorage sharedHTTPCookieStorage] deleteCookie:cookie];
    }
}

- (void)onApplicationOpenURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication
{
    [WXApi handleOpenURL:url delegate:self];
}

- (void)shareToWechatWithTitle:(NSString *)aTitleString content:(NSString *)aContentString thumbImagePath:(NSString *)aThumbImagePath url:(NSString *)aUrlString scene:(int)aScene
{
    SendMessageToWXReq* req = [[[SendMessageToWXReq alloc] init] autorelease];
    WXMediaMessage *message = [WXMediaMessage message];


    // 标题
    [message setTitle:aTitleString];
    
    // 内容
    [message setDescription:aContentString];
    
    NSString *contentString = message.description;
    NSInteger contentLength = [contentString lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
    if (contentLength > 1024)
    {
        for (int i=0; i<6; i++)
        {
            NSString *subString = [[NSString alloc] initWithBytes:[[contentString dataUsingEncoding:NSUTF8StringEncoding] bytes] length:1024-i encoding:NSUTF8StringEncoding];
            [message setDescription:subString];
            [subString release];
            if (message.description!=nil)
            {
                break;
            }
        }
    }
    
    if(aThumbImagePath!=nil && aThumbImagePath.length>0){
        UIImage *thumbImage = nil;
        thumbImage =  [UIImage imageWithContentsOfFile:aThumbImagePath];
        if(thumbImage!=nil){
            [message setThumbImage:thumbImage];
            UIImage *realThumbImage = [self scaledImage:thumbImage ofSize:CGSizeMake(70.0f, 70.0f)];
            NSData *thumbImageData = UIImageJPEGRepresentation(realThumbImage, 0.7f);
            [message setThumbData:thumbImageData];
            if ([message.thumbData length] > (1<<15))
            {
                [message setThumbImage:[UIImage imageNamed:@"Icon"]];
            }
        }
        
    }else{
        [message setThumbImage:[UIImage imageNamed:@"Icon"]];
    }
    

    // 链接
    if (aUrlString.length <=0)
    {
        //aUrlString = KShareGuanwangUrl;
    }
    WXWebpageObject *ext = [WXWebpageObject object];
    [ext setWebpageUrl:aUrlString];
    [message setMediaObject:ext];

    [req setBText:NO];
    [req setMessage:message];
    [req setScene:aScene];

    BOOL isSuccess = [WXApi sendReq:req];
    if (isSuccess == NO)
    {
        //[CustomIndicator showIndicatorOnTimerWithType:Str andString:@"分享失败"];
    }
    else
    {
        //[CustomIndicator hideLoadingView];
    }

}


- (void)shareToWechatWithImagePath:(NSString *)aImagePath scene:(int)aScene
{
    SendMessageToWXReq* req = [[[SendMessageToWXReq alloc] init] autorelease];
    WXMediaMessage *message = [WXMediaMessage message];

    if(aImagePath!=nil&&aImagePath.length>0){
        UIImage *shareImage = nil;
        shareImage =  [UIImage imageWithContentsOfFile:aImagePath];
        if(shareImage!=nil){

            UIImage *thumbImage = [self scaledImage:shareImage ofSize:CGSizeMake(70.0f, 70.0f)];
            NSData *thumbImageData = UIImageJPEGRepresentation(thumbImage, 0.7f);
            [message setThumbData:thumbImageData];
            if ([message.thumbData length] > (1<<15))
            {
                [message setThumbImage:[UIImage imageNamed:@"Icon"]];
            }

            WXImageObject *ext = [WXImageObject object];
            [ext setImageData:UIImageJPEGRepresentation(shareImage, 1.0f)];
            [message setMediaObject:ext];
        }


    }
    [req setBText:NO];
    [req setMessage:message];
    [req setScene:aScene];
    
    BOOL isSuccess = [WXApi sendReq:req];
    if (isSuccess == NO)
    {
        //[CustomIndicator showIndicatorOnTimerWithType:Str andString:@"分享失败"];
    }
    else
    {
        //[CustomIndicator hideLoadingView];
    }

}

- (void)shareToWechat:(WechatMessageType)aMessageType titleString:(NSString *)aTitleString content:(NSString *)aContentString url:(NSString *)aUrlString scene:(int)aScene
{
    WXMediaMessage *message = [WXMediaMessage message];
    SendMessageToWXReq* req = [[[SendMessageToWXReq alloc] init]autorelease];
    if(aMessageType == WXMESSAGE_TYPE_TEXT){
        req.scene = aScene;
        req.bText = YES;
        req.text = aTitleString;
        [WXApi sendReq:req];
        return;
    }
    else if(aMessageType == WXMESSAGE_TYPE_IMG){
        if(aUrlString!=nil&&aUrlString.length>0){
            UIImage *shareImage = nil;
            shareImage =  [UIImage imageWithContentsOfFile:aUrlString];
            if(shareImage!=nil){

                UIImage *thumbImage = [self scaledImage:shareImage ofSize:CGSizeMake(70.0f, 70.0f)];
                NSData *thumbImageData = UIImageJPEGRepresentation(thumbImage, 0.7f);
                [message setThumbData:thumbImageData];
                if ([message.thumbData length] > (1<<15))
                {
                    [message setThumbImage:[UIImage imageNamed:@"Icon"]];
                }

                WXImageObject *ext = [WXImageObject object];
                [ext setImageData:UIImageJPEGRepresentation(shareImage, 1.0f)];
                [message setMediaObject:ext];
            }
        }else{
            return;
        }
        [req setBText:NO];
    }
    else if(aMessageType == WXMESSAGE_TYPE_MUSIC){
        [message setTitle:aTitleString];
        [message setDescription:aContentString];
        [message setThumbImage:[UIImage imageNamed:@"Icon.jpg"]];      
        WXMusicObject *ext = [WXMusicObject object];
        ext.musicDataUrl = aUrlString;
        message.mediaObject = ext;  
        req.bText = NO;
    }
    else if(aMessageType == WXMESSAGE_TYPE_VIDEO){
        [message setTitle:aTitleString];
        [message setDescription:aContentString];
        [message setThumbImage:[UIImage imageNamed:@"Icon.jpg"]];  
        WXVideoObject *ext = [WXVideoObject object];
        ext.videoUrl = aUrlString;
        message.mediaObject = ext;
        req.bText = NO;
    }
    else if(aMessageType == WXMESSAGE_TYPE_WEBPAGE){
        [message setTitle:aTitleString];
        [message setDescription:aContentString];
        [message setThumbImage:[UIImage imageNamed:@"Icon.jpg"]];
        WXWebpageObject *ext = [WXWebpageObject object];
        ext.webpageUrl = aUrlString;
        message.mediaObject = ext;
        req.bText = NO;
    } 
    req.message = message;
    req.scene = aScene;
    BOOL isSuccess = [WXApi sendReq:req];
    if(isSuccess == NO){

    }else{

    }
}

// wechat delegate
- (void)onResp:(BaseResp*)resp
{
    NSString *strErrorCode = [NSString stringWithFormat:@"%d", resp.errCode];
    [self sendMessage:@"Weixin" message:@"onResp" param:strErrorCode];
}

- (void)sendMessage:(NSString*)aGameObject message:(NSString*)aMessage param:(NSString*)aParam
{
	UnitySendMessage( [aGameObject UTF8String], [aMessage UTF8String], [aParam UTF8String] );
}

@end

void _LoadUrl(const char *url, int platform)
{
    NSString *oauthUrl = [NSString stringWithUTF8String:url];
    PlatformType pt = (PlatformType)platform;
    [[UniSocialNativeToolkit sharedManager] loadUrl:oauthUrl platformType:pt];
}

void _Logout(int platform)
{
    PlatformType pt = (PlatformType)platform;
    [[UniSocialNativeToolkit sharedManager] logout:pt]; 
}

void _registerWeixinApi(const char *urlSchemes)
{
    [WXApi registerApp:[NSString stringWithUTF8String:urlSchemes]];
}

bool _isWechatAvailable()
{
    return [WXApi isWXAppInstalled] && [WXApi isWXAppSupportApi];
}

void _shareToWechatWithText(const char *titleString, const char *content, const char *thumbImagePath, const char *url, int scene)
{
    NSString *nstitleString = [NSString stringWithUTF8String:titleString];
    NSString *nsContent = [NSString stringWithUTF8String:content];
    NSString *nsThumbImagePath = [NSString stringWithUTF8String:thumbImagePath];
    NSString *nsUrl = [NSString stringWithUTF8String:url];
    [[UniSocialNativeToolkit sharedManager] shareToWechatWithTitle:nstitleString content:nsContent thumbImagePath:nsThumbImagePath url:nsUrl scene:scene];
}

void _shareToWechatWithImage(const char *imagePath, int scene)
{
    NSString *nsImagePath = [NSString stringWithUTF8String:imagePath];
    [[UniSocialNativeToolkit sharedManager] shareToWechatWithImagePath:nsImagePath scene:scene];

}

void _shareToWechat(int msgType, const char *titleString, const char *content, const char *thumbImagePath, const char *url, int scene)
{
    WechatMessageType messageType = (WechatMessageType)msgType;
    NSString *nstitleString = [NSString stringWithUTF8String:titleString];
    NSString *nsContent = [NSString stringWithUTF8String:content];
    NSString *nsUrl = [NSString stringWithUTF8String:url];    
    [[UniSocialNativeToolkit sharedManager] shareToWechat:messageType titleString:nstitleString content:nsContent url:nsUrl scene:scene];
}
