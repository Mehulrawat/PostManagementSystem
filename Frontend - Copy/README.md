# Post Management Frontend (Angular)

Minimal Angular frontend for the Post Management System.

## Setup

1. Install Node.js 24 / npm 11 and Angular CLI (20.3.8).
2. In this folder:

```bash
npm install
npx ng serve
```

3. Make sure the backend API is running at `https://localhost:7250` or update `src/environments/environment.ts`.

Features (auth required for `/api`):
- Login / Register
- Create draft post
- Submit draft for approval
- List approved/resolved posts
- My posts (with Submit for Approval action)
- Post detail with comments
- Admin dashboard (approve/reject/resolve/close posts)
