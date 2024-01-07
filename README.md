# QPM-verification

1. Open qpm.e2e solution
2. Rebuild
3. Run the AdminCreatesPIsAndEpics_UserVerifiesData test in the SanityTests class
4. The test is based on the Chronium browser, if it not present on yor PC, you may see next error:
 
 Looks like Playwright was just installed or updated.        ║
 ║ Please run the following command to download new browsers: ║
 ║                                                            ║
 ║     pwsh bin/Debug/net8.0/playwright.ps1 install           ║
 ║                                                            ║
 ║ <3 Playwright Team   

To fix it, just run playwright.ps1 install that is located in the mentioned folder.
