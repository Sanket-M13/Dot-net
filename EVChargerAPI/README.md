# EVChargerAPI - Railway Deployment

## Quick Deploy to Railway

1. **Connect to Railway:**
   - Go to [railway.app](https://railway.app)
   - Sign up/Login with GitHub
   - Click "New Project" → "Deploy from GitHub repo"
   - Select this repository

2. **Set Environment Variables:**
   ```
   DATABASE_URL=your_database_connection_string
   JWT_SECRET_KEY=your-super-secret-jwt-key-that-is-at-least-32-characters-long
   JWT_ISSUER=EVChargerAPI
   JWT_AUDIENCE=EVChargerClient
   ASPNETCORE_ENVIRONMENT=Production
   ```

3. **Database Setup:**
   - Add PostgreSQL or MySQL service in Railway
   - Copy the connection string to `DATABASE_URL`
   - Update `Program.cs` if using PostgreSQL (install Npgsql.EntityFrameworkCore.PostgreSQL)

## Docker Hub Deployment

```bash
# Login to Docker Hub
docker login

# Build the image (replace 'yourusername' with your Docker Hub username)
docker build -t yourusername/evcharger-api:latest .

# Push to Docker Hub
docker push yourusername/evcharger-api:latest

# Run locally to test
docker run -p 8080:8080 yourusername/evcharger-api:latest
```

## Railway CLI Deployment (Alternative)

```bash
# Install Railway CLI
npm install -g @railway/cli

# Login and deploy
railway login
railway link
railway up
```

## Environment Variables Needed:

- `DATABASE_URL`: Database connection string
- `JWT_SECRET_KEY`: JWT signing key (32+ characters)
- `JWT_ISSUER`: JWT issuer name (e.g., EVChargerAPI)
- `JWT_AUDIENCE`: JWT audience name (e.g., EVChargerClient)
- `AllowedOrigins__0`: First allowed origin (e.g., https://your-frontend.vercel.app)
- `AllowedOrigins__1`: Second allowed origin (optional)
- `AllowedOrigins__2`: Third allowed origin (optional)

## Notes:
- The app runs on port 8080 (Railway standard)
- Health check endpoint: `/health` (if implemented)
- Swagger UI available at `/swagger` in development