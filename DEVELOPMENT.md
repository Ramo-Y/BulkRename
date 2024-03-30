# Description
You are welcome to participate in the development of this tool, in this file some information and rules for the development are described.

# Documentation
If changes are made to the interface or functionality, the screenshots and documentation should also be updated.

# Branches
To contribute, please create a fork of the repository, work on a feature branch and put a pull request in the `develop` branch. Once ci has run successfully with the tests, it can be merged into the `develop` branch. The `release` branch is created from the `master` branch and [semantic versioning](https://semver.org) is used.

# Workflows
The workflows are described in the following table:

| Workflow       | File name                                                                              | Trigger                                              | Description                                                                            |
|----------------|----------------------------------------------------------------------------------------|------------------------------------------------------|----------------------------------------------------------------------------------------|
| ci             | [build-test.yml](./.github/workflows/build-test.yml)                                   | All branches and pull requests to develop and master | Builds and tests the software                                                          |
| release        | [build-test-release.yml](./.github/workflows/build-test-release.yml)                   | Commits to develop, master and release/* branches    | Builds and tests software and pushes it to the Docker Hub if the tests were successful |
| manual_release | [build-test-release_manually.yml](./.github/workflows/build-test-release_manually.yml) | Manual, can be triggeret on any branch               | Builds and tests software and pushes it to the Docker Hub if the tests were successful |

Version names are automatically set by the reusable workflow `set_version`, based on branch names. See in the file [set_version.yml](./.github/workflows/set_version.yml)

## Variables and secrets
In order for the workflows to run on your GitHub account, the following variables and secrets must be set:

| Name                   | Type     | Description                                                                                                                                                                 |
|------------------------|----------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| IMAGE_NAME             | Variable | Name of the image, e.g. bulkrename                                                                                                                                          |
| SQL_SERVER_SA_PASSWORD | Secret   | Password for SA user, refer also to [Password Policy](https://learn.microsoft.com/en-us/sql/relational-databases/security/password-policy?view=sql-server-ver16)            |
| DOCKERHUB_USERNAME     | Secret   | Used in the release workflow for docker login                                                                                                                               |
| DOCKERHUB_TOKEN        | Secret   | Is used in the release workflow for the Docker login, must be created first, see [Docker Hub documentation](https://docs.docker.com/security/for-developers/access-tokens/) |

# .env file
In order that parameter values are not directly in the docker-compose.yml, the .env file is used here. This allows the parameters and values to be specified as a key-value pair in this file. The [CreateEnvFile.ps1](./src/CreateEnvFile.ps1) script was created to avoid having to create the file manually. If new parameters are defined for the docker-compose.yml file, the script should be extended accordingly.

# Version
The version is set in the following files:
- VERSION in [Dockerfile](./src/Dockerfile)
- VersionPrefix in [Directory.Build.props](./src/Directory.Build.props)
- Parameter `-Version` in [CreateEnvFile.ps1](./src/CreateEnvFile.ps1)

# Database fields
### Primary key
Primary keys of a table have a suffix `ID` after the table name. For example, the table `Episode` has the field `EpisodeID` as GUID.

### All fields
All fields except the primary key fields follow a prefix with the shortened 3-letter name of the table. For the table `Episode` we can use `Eps` for the field `EpsEpisodeName`.

### Foreign Keys
Foreign key fields follow the given pattern:
- `Eps` as the general preffix
- `SeasonID` same name as the primary key field of the referenced table
- `_FK` as a suffix to highlight that it is a foreign key

# Working locally
## Local build
To build the docker image locally, you can run the command `docker build -t bulkrename:latest .` in the root folder of the solution.

## Running the container
As long as you have an .env file in the root directory of the solution, run `docker-compose up -d` and the container will be started together with the database. To establish a database connection, a value of the environment variable `SqlServerSaPassword` must be defined manually and the `PersistanceMode` must be set to Database. Also check whether the mapped folder paths exist on your computer, by default everything is mapped to the `D:\` drive, but can be changed.
