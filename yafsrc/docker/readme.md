Build a docker image: 

```bash
$> docker-compose -f docker-compose-SqlServer.yml up --build
```
Run a container with: 

```bash
$> docker run --rm -it -p 8080:80 yafnet
```

```bash
$> docker-compose -f docker-compose-SqlServer.yml up --build
```

Open your browser and navigate to: localhost:8080