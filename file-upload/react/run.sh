#!/usr/bin/env bash

set -e

PROJ_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/"
cd "$PROJ_ROOT"

yarn install
yarn start
