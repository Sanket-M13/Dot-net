# Railway Deployment Checklist ✅

## Pre-Deployment Verification

### ✅ 1. DLL Name Verification
- [x] DLL name is: **EVChargerAPI.dll**
- [x] Matches ENTRYPOINT in Dockerfile

### ✅ 2. Program.cs Check
- [x] Uses `app.Run()` (no hardcoded port)
- [x] CORS configured with environment variables

### ✅ 3. Dockerfile Validation
- [x] Multi-stage build
- [x] Port 8080 exposed
- [x] ASPNETCORE_URLS set to http://0.0.0.0:8080
- [x] Correct ENTRYPOINT: `dotnet EVChargerAPI.dll`

## Railway Environment Variables

Set these in Railway Dashboard:

```bash
# Database
DATABASE_URL=Server=your-server;Database=EVChargerDB;User Id=user;Password=pass;TrustServerCertificate=True

# JWT Configuration
JWT_SECRET_KEY=your-super-secret-jwt-key-minimum-32-characters-long-for-security
JWT_ISSUER=EVChargerAPI
JWT_AUDIENCE=EVChargerClient

# CORS Origins (add your frontend URLs)
AllowedOrigins__0=https://your-frontend.vercel.app
AllowedOrigins__1=https://your-frontend.netlify.app
AllowedOrigins__2=https://your-custom-domain.com

# Environment
ASPNETCORE_ENVIRONMENT=Production
```

## Deployment Steps

1. **Push to GitHub**
   ```bash
   git add .
   git commit -m "Add Docker configuration for Railway"
   git push origin main
   ```

2. **Connect Railway**
   - Go to railway.app
   - New Project → Deploy from GitHub
   - Select your repository
   - Select EVChargerAPI folder as root

3. **Add Database** (if needed)
   - Add PostgreSQL or MySQL service
   - Copy connection string to DATABASE_URL

4. **Set Environment Variables**
   - Add all variables listed above
   - Save changes

5. **Deploy**
   - Railway auto-deploys on push
   - Check logs for: `Now listening on: http://0.0.0.0:8080`

## Success Indicators

✅ Deployment shows "ACTIVE"
✅ Logs show: `Now listening on: http://0.0.0.0:8080`
✅ Logs show: `Application started`
✅ URL accessible: `https://your-app.up.railway.app/swagger`

## Troubleshooting

### Container exits immediately
- Check DLL name matches exactly
- Verify DATABASE_URL is correct
- Check Railway logs for errors

### CORS errors
- Add your frontend URL to AllowedOrigins
- Format: `AllowedOrigins__0=https://your-url.com`

### Database connection fails
- Verify DATABASE_URL format
- Add `TrustServerCertificate=True` for SQL Server
- Check database service is running

## Test Endpoints

```bash
# Health check (if implemented)
curl https://your-app.up.railway.app/health

# Swagger UI
https://your-app.up.railway.app/swagger

# API endpoint
curl https://your-app.up.railway.app/api/stations
```