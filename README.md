![Static Badge](https://img.shields.io/badge/Prod_Version-1.4-green)  
![Static Badge](https://img.shields.io/badge/Dev_Version-1.5-yellow)




### CI/CD Plan  
#### Main branch should be forked and merged to only for necessary hotfixes.  
#### Dev branch will be pushed to production server monthly on the first Friday.
#### Dev branch should be forked for all feature updates and non-critical bugfixes  
#### Any changes made directly to main branch will trigger an automatic downmerge to dev, if you need a clean copy of dev without this downmerge, branch from dev before pushing to main  

### Feature Contribution
1. Create an issue in [the project](https://github.com/users/Malechus/projects/2) or pick an issue you'd like to work on
2. Assign the issue to yourself
3. Assign a priority and size
4. Open a new branch **from dev** through the issue pane
5. Checkout your new branch locally
6. Make your changes and commit to your branch
7. Open a PR to merge your branch into dev

### App config
#### This webapp requires config files not included in this repo to protect sensitive account information. There is a copy in the codespace secrets of this project. To obtain these files contact Malechus at [malechus@fangandclaw.org.](mailto:malechus@fangandclaw.org)  

### Server info
#### Web server 66.228.34.152
#### Prod site *.kicevents.com, cure.kicevents.com
#### Dev site dev.kicevents.com, dev.cure.kicevents.com
