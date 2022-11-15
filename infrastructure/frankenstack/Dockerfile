FROM node:14

WORKDIR /usr/src/app

COPY package*.json ./

RUN apt-get update && apt-get install curl less -y

RUN npm install -g frankenstack && \
    npm install

RUN curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip" && \
    unzip awscliv2.zip && \
    ./aws/install && \
    aws --version

ENV TZ=UTC7

CMD [ "frank.sh" ]
