# QPM-verification

1. Open qpm.e2e solution
2. Rebuild
3. Run the AdminCreatesPIsAndEpics_UserVerifiesData test in the SanityTests class
4. The test is based on the Chronium browser, if it not present on yor PC, you may see " Looks like Playwright was just installed or updated." error
 in such case you need to follow the error description and run the  pwsh bin/Debug/net8.0/playwright.ps1 install to install it.
