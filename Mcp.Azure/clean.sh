#!/bin/bash

# Find and remove all bin and obj folders
find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +

# Also remove .vs folder if it exists
rm -rf .vs

echo "Cleaned all bin, obj, and .vs folders" 