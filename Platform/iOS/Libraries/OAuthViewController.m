//
//  OAuthViewController.m
//  UniSocial
//
//  Created by Ren Chonghui on 11/20/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "OAuthViewController.h"

void UnitySendMessage( const char * obj, const char * method, const char * msg );

@implementation OAuthViewController
@synthesize webView;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (id)initWithUrl:(NSString *)aUrl platform:(PlatformType)aPlatformType
{
    self = [super init];
    if(self) {
        self.oauthUrl = aUrl;
        self.platformType = aPlatformType;
    }

    return self;
}
- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

// Implement loadView to create a view hierarchy programmatically, without using a nib.
- (void)loadView {
    [super loadView];
    CGRect rect = self.view.bounds;
    UIWebView *wv = [[UIWebView alloc] initWithFrame:rect];
    self.webView = wv;
    [wv release];
    self.webView.delegate = self;
    self.webView.autoresizingMask = UIViewAutoresizingFlexibleWidth|UIViewAutoresizingFlexibleHeight;
	[self.view addSubview:self.webView];
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    self.platformNameArray = [NSArray arrayWithObjects:@"新浪微博", @"腾讯微博", @"人人网", @"开心", @"豆瓣", @"微信", @"Facebook", @"Twitter", @"Linkedin", @"Foursquare", @"GooglePlus", @"QQ空间", nil];
    self.platformGameObjectArray = [NSArray arrayWithObjects:@"SinaWeibo", @"TencentWeibo", @"Renren", @"Kaixin", @"Douban", @"Weixin", @"Facebook", @"Twitter", @"Linkedin", @"Foursquare", @"GooglePlus", @"QZone", nil];
    
    //self.navigationItem.hidesBackButton = YES;
    self.title = [self.platformNameArray objectAtIndex:(int)self.platformType];
    UIBarButtonItem *left = [[UIBarButtonItem alloc] initWithTitle:@"Cancel" style:UIBarButtonSystemItemCancel target:self action:@selector(cancel:)];
    self.navigationItem.leftBarButtonItem = left;
    [left release];
    
    UIActivityIndicatorView *aiv = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    self.waitingIndicator = aiv;
    [aiv release];

    self.waitingIndicator.center = CGPointMake(self.view.bounds.size.width / 2, self.view.bounds.size.height / 2);
    [self.waitingIndicator setAutoresizingMask:UIViewAutoresizingFlexibleLeftMargin|UIViewAutoresizingFlexibleRightMargin|UIViewAutoresizingFlexibleTopMargin|UIViewAutoresizingFlexibleBottomMargin];
    [self.view addSubview:self.waitingIndicator];
    [self.waitingIndicator setHidesWhenStopped:YES];
    [self.waitingIndicator startAnimating];

    [self.view bringSubviewToFront:self.waitingIndicator];
    
    NSURLRequest *request =[NSURLRequest requestWithURL:[NSURL URLWithString:self.oauthUrl]
                                            cachePolicy:NSURLRequestReloadIgnoringLocalCacheData
                                        timeoutInterval:60.0];
    [self.webView loadRequest:request];
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
    [self setWebView:nil];
    [self setWaitingIndicator:nil];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)dealloc {
    [self.webView release];
    [_oauthUrl release];
    [_platformGameObjectArray release];
    [_platformNameArray release];
    [_waitingIndicator release];
    [super dealloc];
}

#pragma mark - UIWebView delegate
- (BOOL)webView:(UIWebView *)aWebView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
	if(request==nil){
		return YES;
	}
    NSString *strUrl = request.URL.absoluteString;
    NSLog(@"url=%@", request.URL.absoluteString);
    NSRange range = [strUrl rangeOfString:@"code="];
    if(range.location!=NSNotFound){
		int pt = (int)self.platformType;
        //self.platformGameObjectArray[pt]
        [self sendMessage:[self.platformGameObjectArray objectAtIndex:pt] message:@"AuthCallback" param:[strUrl substringFromIndex:range.location]];
        [self dismissModalViewControllerAnimated:YES];
		return YES;
	}
	
	range = [strUrl rangeOfString:@"access_token"];
    if(range.location!=NSNotFound){
        int pt = (int)self.platformType;
        //self.platformGameObjectArray[pt]
        [self sendMessage:[self.platformGameObjectArray objectAtIndex:pt] message:@"AuthCallback" param:[strUrl substringFromIndex:range.location]];
        [self dismissModalViewControllerAnimated:YES];
		return YES;
    }
	range = [strUrl rangeOfString:@"oauth_verifier"];
	if(range.location!=NSNotFound){
		int pt = (int)self.platformType;
		range = [strUrl rangeOfString:@"oauth_token"];
		[self sendMessage:[self.platformGameObjectArray objectAtIndex:pt] message:@"AuthCallback" param:[strUrl substringFromIndex:range.location]];
		[self dismissModalViewControllerAnimated:YES];
	}
    return YES;
}

- (void)webViewDidFinishLoad:(UIWebView *)webView
{
    [self.waitingIndicator stopAnimating];
}

//oAuth2
- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error{
    [self.waitingIndicator stopAnimating];
}

#pragma mark Action
- (void)cancel:(id)sender
{
    int pt = (int)self.platformType;
	[self sendMessage:[self.platformGameObjectArray objectAtIndex:pt] message:@"AuthCallback" param:@"UserCancel"];
    [self dismissModalViewControllerAnimated:YES];
}

- (void)sendMessage:(NSString*)aGameObject message:(NSString*)aMessage param:(NSString*)aParam
{
	UnitySendMessage( [aGameObject UTF8String], [aMessage UTF8String], [aParam UTF8String] );
}
@end
