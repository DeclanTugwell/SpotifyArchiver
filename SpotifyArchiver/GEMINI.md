# SpotifyArchiver

## Project Overview

The project to be developed is a three layer Command Query Responsibility Segregation (CQRS) console application called Spotify Archiver. The presentation layer provides a terminal interface for all user interaction, the application layer handles interactions with the Spotify API, and the data access layer handles data persistence using a local database.

## Solution Structure

Staircase pattern is being used. The solution contains the following assemblies:

SpotifyArchiver.Presentation

SpotifyArchiver.Application.Abstraction
SpotifyArchiver.Application.Implementation

SpotifyArchiver.DataAccess.Abstraction
SpotifyArchiver.DataAccess.Implementation

## Key Solution rules

Credentials are stored in environment variables, these should be used for any integration tests and for the deployed solution:

- SPOTIFY_CLIENT_ID
- SPOTIFY_REDIRECT_URI

CQRS and CLEAN architectures should be adhered to with asymmetric layering being used with the fastest layering traversal.
Abstraction assemblies should have no dependancies used by the implementation whether third or first party.
