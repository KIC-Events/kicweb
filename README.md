### CI/CD Plan  
#### Main branch should be forked and merged to only for necessary hotfixes.  
#### Main branch will be pushed to production server on **alternate Fridays**  
#### Dev branch will be merged into main following push  
#### Dev branch should be forked for all feature updates and non-critical bugfixes  
#### Any changes made directly to main branch will trigger an automatic downmerge to dev, if you need a clean copy of dev without this downmerge, branch from dev before pushing to main  

### App config
#### This webapp requires config files not included in this repo to protect sensitive account information. To obtain these files contact Malechus at [malechus@fangandclaw.org.](mailto:malechus@fangandclaw.org)  

### Server info
#### Prod server 66.228.34.152
