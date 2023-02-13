#!/bin/sh
 
echo "Replace NEXT_PUBLIC_AZURE_AD_CLIENT_ID vars"
test -n "NEXT_PUBLIC_AZURE_AD_CLIENT_ID"
 
find /app/.next/standalone \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -i "s#REPLACE_NEXT_PUBLIC_AZURE_AD_CLIENT_ID#$NEXT_PUBLIC_AZURE_AD_CLIENT_ID#g"

echo "Replace NEXT_PUBLIC_AZURE_AD_TENANT_ID vars"
test -n "NEXT_PUBLIC_AZURE_AD_TENANT_ID"
 
find /app/.next/standalone \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -i "s#REPLACE_NEXT_PUBLIC_AZURE_AD_TENANT_ID#$NEXT_PUBLIC_AZURE_AD_TENANT_ID#g"

echo "Replace NEXT_PUBLIC_AZURE_AD_TENANT_NAME vars"
test -n "NEXT_PUBLIC_AZURE_AD_TENANT_NAME"
 
find /app/.next/standalone \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -i "s#REPLACE_NEXT_PUBLIC_AZURE_AD_TENANT_NAME#$NEXT_PUBLIC_AZURE_AD_TENANT_NAME#g"

echo "Replace NEXT_PUBLIC_AZURE_AD_USER_FLOW vars"
test -n "NEXT_PUBLIC_AZURE_AD_USER_FLOW"
 
find /app/.next/standalone \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -i "s#REPLACE_NEXT_PUBLIC_AZURE_AD_USER_FLOW#$NEXT_PUBLIC_AZURE_AD_USER_FLOW#g"

echo "Replace NEXT_PUBLIC_AZURE_AD_USER_FLOW vars"
test -n "NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING"
 
find /app/.next/standalone \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -i "s#REPLACE_NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING#$NEXT_PUBLIC_APPLICATIONINSIGHTS_CONNECTION_STRING#g"
 
echo "Starting Nextjs"
exec "$@"