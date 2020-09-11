# Domain MX to IPs resolving

## About

App is targeting .NET Core 3.1. DnsClient (https://dnsclient.michaco.net/) is handling DNS truncated answers pretty good. Built-in simple cache to not ask for the same thing many times.

## Usage

  -i, --inputfile     (Group: domainsinput) Domains to be checked in a file, each domain in separate line

  -d, --domains       (Group: domainsinput) Space separated list of domains to check

  -s, --dns           Required. DNS server to use for checks

  -o, --outputfile    Output file, if not specified output on console

Example:

SampleDNSResolve.exe -d yahoo.com google.com -s 8.8.8.8

SampleDNSResolve.exe -d yahoo.com google.com -s 8.8.8.8 -o output.txt

SampleDNSResolve.exe -i input.txt -s 8.8.8.8 -o output.txt

## Output format

Each output line contains:
- DNS Server used for checks,
- Domain name that is being checked,
- IP (or IPs) of MX servers,
- MX Preference number.
