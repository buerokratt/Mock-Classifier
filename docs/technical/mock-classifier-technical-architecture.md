# Mock Classifier Technical Architecture

This document outlines the technical design for the mock classifier component.

## Introduction

Classifier has a single responsibility which is to determine which Ministry can answer the query from a user via a bot.

The Classifier takes raw messages from user's as an input and returns the Ministry which can answer these message as an output.

The Classifier achieves this objective via an AI-based language model featuring intent matching and entity extraction. The Classifier will be pre-trained to know how to resolve to these 11 ministries.

The design and build of the real Classifier will be undertaken in parallel to the main project by a separate team. For the purposes of the project, we will create a mock Classifier which takes the same inputs as the real service and produces the same outputs, but has hard-coded resolutions.

The mocked Classifier Service will offer two main features:

- For demo, we will match hard coded strings (in English) to Ministries. For example "Thanks. How do I register my daughter at a local school?" would return "Ministry of Education and Research" as the Ministry
- For testing and broader scenarios, we will also have a token format where we can pass in the Ministries we want to return

Once the appropriate Ministry has been resolved by Classifier, DMR will consult CentOps to map the Ministry to a specific Bot uri.

## Ministries

The classifier will return one of the 11 central government ministries as listed in the top navigation of https://www.valitsus.ee/en.

The actual text returned will be the key word from the full name in lower case (referred to as a "label").

| Ministry Name                                                | Label       |
| :----------------------------------------------------------- | ----------- |
| [Ministry of Justice](https://www.just.ee/en)                | justice     |
| [Ministry of Education and Research](https://www.hm.ee/en)   | education   |
| [Ministry of Defence](https://www.kaitseministeerium.ee//en) | defence     |
| [Ministry of the Environment](https://www.envir.ee/en)       | environment |
| [Ministry of Cultural Affairs](https://www.kul.ee/en)        | cultural    |
| [Ministry of Rural Affairs](https://www.agri.ee/en)          | rural       |
| [Ministry of Economic Affairs and Communications](https://www.mkm.ee/en) | economic    |
| [Ministry of Finance](https://www.rahandusministeerium.ee/en) | finance     |
| [Ministry of Social Affairs](https://www.sm.ee/en)           | social      |
| [Ministry of the Interior](https://www.siseministeerium.ee/en) | interior    |
| [Ministry of Foreign Affairs](https://vm.ee/en)              | foreign     |

## Natural language resolutions

## Token Resolutions

The Classifier will accept special tokens anywhere in the input text which will inform the Classifier which ministry to resolve.

The token format will be `{{token}}` where token is one of labels listed in the Ministries section.

Here are some examples:

- An input text of `Please return the {{social}} minsitry` will return `social`
- An input text of `{{environment}}` will return `environment`
- An input text of `{{rural}} please` will return `rural`

Spaces are not permitted so an input text of `Please return {{ environment}}` would return an error code.