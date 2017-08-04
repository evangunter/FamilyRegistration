# FamilyRegistration
The Family Registration page streamlines registration for new families in your church community. Not only can you register new families, but you can add children to existing families.

Getting Started
--------
The Family Registration page has been customized for my church - this includes branding, style, and overall look and feel. You probably want to change some of this, if not everything.

*Note, after a person completes step 1 of registering their new family (adding adults), my intention was to display a list of children in the family (if they had any) on both the add children page and the registration complete page, BUT I didn't think I could search for people by familyId using the ArenaAPI (because it would make sense to put it here). However, I discovered Arena has a dedicated endpoint just to get family members? I mean I guess thats fine, I just feel like it would have been super easy to add the to their list people method and make familyId a filter - but what do I know?*

### Prereqs
  1). The FamilyRegistration page requires the [Arena.NET package](https://github.com/evangunter/Arena.NET) 
  
  2). The FamilyRegistration project uses an API helper (static class, you just need to define your api setting in the web.config
  ```xml
<add key="Arena_APIUrl" value="https://yourapi.myshelby.org" />
<add key="Arena_Username" value="username" />
<add key="Arena_Password" value="password" />
<add key="Arena_APIKey" value="key" />
<add key="Arena_APISecret" value="secret" />
```
Screenshots
--------
![Screenshot](https://raw.githubusercontent.com/evangunter/FamilyRegistration/master/FamilyRegistration/Content/images/screencapture1.png)
![Screenshot](https://raw.githubusercontent.com/evangunter/FamilyRegistration/master/FamilyRegistration/Content/images/screencapture2.png)
![Screenshot](https://raw.githubusercontent.com/evangunter/FamilyRegistration/master/FamilyRegistration/Content/images/screencapture3.png)
