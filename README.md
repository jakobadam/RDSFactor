# RDSFactor

Two-factor authentication for Remote Desktop Services (RDS)

http://www.isager.dk/is/CICRadarR/SMStokenforWindows2012RDGateway.aspx

## Prerequisites

An RDS setup. The minimal RDS setup for use with RDSFactor consist of two servers: 
* Active Directory; and
* RDS with Gateway component enabled

## Installation

### RDWeb update
RDSfactor comes with a customized version of the RDWeb pages. To install these run:

```
$ install-web.bat
```
### RADIUS server installation

The RADIUS server component can be installed on any server reacheable by both the RD Web and the RD Gateway. To install the server as a service run:

```
$ install-server.bat
```

TODO: NPS config, Web config

## Acknowledgements

* Claus Isager - for the proof of concept two factor RDS authentication 
* Nikolay Semov - for the core RADIUS server 
