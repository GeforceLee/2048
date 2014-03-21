//
//  OAuthViewController.h
//  UniSocial
//
//  Created by Ren Chonghui on 11/20/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "UniSocialNativeToolkit.h"
@interface OAuthViewController : UIViewController <UIWebViewDelegate>

@property (retain, nonatomic) IBOutlet UIWebView *webView;
@property (nonatomic, retain) NSString *oauthUrl;
@property (nonatomic, assign) PlatformType platformType;
@property (nonatomic, retain) NSArray *platformNameArray;
@property (nonatomic, retain) NSArray *platformGameObjectArray;
@property (nonatomic, retain) UIActivityIndicatorView *waitingIndicator;
- (id)initWithUrl:(NSString *)aUrl platform:(PlatformType)aPlatformType;
@end
