#!/bin/sh
 
echo "Check that we have NEXT_PUBLIC_AZURE_AD_CLIENT_ID vars"
test -n "NEXT_PUBLIC_AZURE_AD_CLIENT_ID"
 
find /app/.next \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -i "s#APP_NEXT_PUBLIC_AZURE_AD_CLIENT_ID#$NEXT_PUBLIC_AZURE_AD_CLIENT_ID#g"

echo "Check that we have NEXT_PUBLIC_AZURE_AD_TENANT_ID vars"
test -n "NEXT_PUBLIC_AZURE_AD_TENANT_ID"
 
find /app/.next \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -i "s#APP_NEXT_PUBLIC_AZURE_AD_TENANT_ID#$NEXT_PUBLIC_AZURE_AD_TENANT_ID#g"

echo "Check that we have NEXT_PUBLIC_AZURE_AD_TENANT_NAME vars"
test -n "NEXT_PUBLIC_AZURE_AD_TENANT_NAME"
 
find /app/.next \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -i "s#APP_NEXT_PUBLIC_AZURE_AD_TENANT_NAME#$NEXT_PUBLIC_AZURE_AD_TENANT_NAME#g"

echo "Check that we have NEXT_PUBLIC_AZURE_AD_USER_FLOW vars"
test -n "NEXT_PUBLIC_AZURE_AD_USER_FLOW"
 
find /app/.next \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -i "s#APP_NEXT_PUBLIC_AZURE_AD_USER_FLOW#$NEXT_PUBLIC_AZURE_AD_USER_FLOW#g"
 
echo "Starting Nextjs"
exec "$@"