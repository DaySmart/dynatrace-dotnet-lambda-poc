#!/usr/bin/env bash

echo Building frank image.
docker build -t q/frank .

echo Executing frank image for $1
docker run --rm -it -e AWS_PROFILE=${AWS_PROFILE:-153033334262_FrankenstackUser} \
        -v $HOME/.aws/:/root/.aws/:ro \
        -v $(pwd)/frank.sh:/usr/src/app/frank.sh:ro \
        -v $(pwd)/envs/:/usr/src/app/envs/:ro \
        --entrypoint=/bin/bash \
        q/frank \
        frank.sh $1
