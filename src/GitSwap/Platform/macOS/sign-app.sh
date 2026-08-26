#!/bin/bash
# Script to sign the macOS app bundle properly
# Usage: ./sign-app.sh <path-to-app-bundle>

APP_BUNDLE="$1"

if [ -z "$APP_BUNDLE" ]; then
    echo "Usage: $0 <path-to-app-bundle>"
    exit 1
fi

if [ ! -d "$APP_BUNDLE" ]; then
    echo "Error: App bundle not found: $APP_BUNDLE"
    exit 1
fi

# Remove any resource fork from executable (causes signing issues)
xattr -cr "$APP_BUNDLE/Contents/MacOS/GitSwap" 2>/dev/null

# Ad-hoc sign the bundle
codesign --force --deep --sign - "$APP_BUNDLE"

if [ $? -eq 0 ]; then
    echo "App bundle signed successfully: $APP_BUNDLE"
else
    echo "Error signing app bundle"
    exit 1
fi
