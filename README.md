# njekyll

This is a custom static blog renderer written in **net5** and based on Jekyll. And it is almost compatible with a Jekyll blog.

## Install

Install latest version using `dotnet` client:

> dotnet tool install --global njekyll --version 0.0.1-alpha4

## Run

You can build your blog site:

> njekyll build

Or you can build and run your site:

> njekyll run

When you are running it locally, it detects blog code changes automatically.

Optionally you can use:

- `-i <folder>` to use a different location than actual as input
- `-o <folder>` to use a different output folder than "_output"

## License

The source code we develop at **njekyll** is default being licensed as CC-BY-SA-4.0. You can read more about [here](LICENSE.md).
