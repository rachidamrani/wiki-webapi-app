# WikiAPI backend application
- Wiki is an online wiki where you can add articles, comments on various topics.

- I chose the JWT standard to secure data and feature access in this application.

- How to test : 

1. Register a user.
2. Create roles using the endpoint : /api/SetupRoles/CreateRole (َ"Admin", "User")
3. Assign a role to the user using the endpoint : /api/SetupRoles/AddUserToRole
4. Hit endpoints with or without a signed user.