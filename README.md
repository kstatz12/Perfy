# Perfy: Dotnet Performance Monitor

This is based on the amazing [realmon](https://github.com/Maoni0/realmon)

## Installation

Global

```
dotnet tool install --global perfy-app --version 0.1.1
```

local

```
dotnet new tool-manifest # if you are setting up this repo
dotnet tool install --local perfy-app --version 0.1.1
```

## Examples

```sh
perfy-app -p 12345 -t 30
```

```sh
pefy-app -n procName -t 30
```

![Sample Output](screenshot.png)
