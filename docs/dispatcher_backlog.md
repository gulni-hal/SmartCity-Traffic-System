1. /health public
2. /api/auth/* public + proxy
3. /api/traffic/* no token => 401
4. /api/fines/* no token => 401
5. token validation via auth service
6. invalid token => 401
7. wrong role => 403
8. downstream unavailable => 502/503
9. audit logging
10. admin action persistence
