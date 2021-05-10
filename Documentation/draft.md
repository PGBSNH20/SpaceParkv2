# Documentation

Use this file to fill in your documentation

# Specifications overview

Big company with several spaceports

API first strategy
    Consistent and re-usable (API description language)
    Automation using tools that allow import API definition files, API-documentation, SDK:s, mock API:s. Swagger Hub.

Multi-Tenant, SaaS-software as a service

Register parkings
Close port when full
Open whens room
Only for spaceships that fit
register in database
Queries done with EF fluent API
Only the REST api have access to DB
Support for more then one spaceport
End goal to distribute
Still exclusive, only star wars residence
API-key with all requests (middleware), alternatively unique keys
database in docker compose
all documentation in file



Users
    Visitor
        Register a parking
        Get information on current parking
        End parking and "pay"
        Get information on all previous parkings

    Admin
        Add new spaceports to the system

identification
    question

registration
    username
    password
    ship

Login
    Enter credentials

Parking
    check available
    enter hours
    confirm parking

Receipts
    Archived receipts

Logic/DB
    Is space port full

Admin User
    Add new spaceport


Get (Header with key):

    users/
        response: all users json
    shipmodels/
        response: all models
    traits/
        response: all user traits
    spaceports/
        response: all spaceport
    pricing/10
        response: price for 10 hours

    user/{id} - info about entity, with parkings
    spaceport/{id} - info about entity, with current parkings

    url: /help/
        documentation

Post:
        url: /register
        Header: secret
        Body:
        {
            "Name": {string},
            "Identification": {
                "Trait": {answer}
            }
            "ShipModelId": int
        }

        url: /park
        Header: secret
        Body:
        {
            "User": 1,
            "SpacePort": 1,
            "EndTime": DateTime
        }

        url: admin/register-spacepark/
        Header: secret
        Body:
        {
            "Name": {string},
            "ShipLimit": {int}
        }
            response: id

delete:
        url: admin/removespaceport/id
        url: admin/removeuser/id