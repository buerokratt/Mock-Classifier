# Mock Classifier Technical Architecture

This document outlines the technical design for the mock classifier component.

## Introduction

Classifier has a single responsibility which is to determine which Ministry can answer the query from a user via a bot.

The Classifier takes raw messages from user's as an input and returns the Ministry which can answer these message as an output.

The design and build of the real Classifier will be undertaken in parallel to the main project by a separate team. For the purposes of the project, we will create a mock Classifier which takes the same inputs as the real service and produces the same outputs, but has hard-coded resolutions.

The mocked Classifier will offer two main features:

- For demo, we will match hard coded strings (in English) to Ministries. For example "Thanks. How do I register my daughter at a local school?" would return "education" as the Ministry
- For testing and broader scenarios, we will also have a token format where we can pass in the exact Ministry we want to return

Once the appropriate Ministry has been resolved by Classifier, DMR, having been provided with appropriate information by CentOps, maps the Ministry to a specific bot uri. It is not Classifier's job to resolve text to bots or institutions, only ministries.

## Ministries

The Classifier will identify one of the 11 Estonian central government ministries as listed in the top navigation of https://www.valitsus.ee/en.

Like all DMR components, the Classifier will not directly respond to the caller, but will instead call back to the DMR with the answer. This will be based on the call back URI defied in the initial message.

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

## Natural Language Resolutions

The Classifier will be hard-coded to support the following natural language resolutions for testing and demo purposes.

| Input Text (all in English)                                  | Calls back with |
| ------------------------------------------------------------ | --------------- |
| I want to register my child at school                        | education       |
| How do I file my annual tax information                      | economic        |
| I have a question about the Estonian pension system          | economic        |
| How do I get to Lahemaa park?                                | environment     |
| How do I arrange for my COVID-19 booster vaccination         | social          |
| I wish to understand what benefits my family are entitled to | social          |

Additional resolutions may be added during the course of implementation as required.

These resolutions will be hard coded into the mock classifier and will not require any external configuration or database.

> This means that if new input text needs to be added, the source code will need to be modified. This is acceptable given that this is a throw-away transient service until the real classifier is ready.

## Token Resolutions

The Classifier will accept special tokens anywhere in the input text which will inform the Classifier which ministries to resolve.

The token format will be `{{token}}` where token is one of labels listed in the Ministries section.

Here are some examples:

- An input text of `Please return the {{social}} minsitry` will call back with `social`
- An input text of `{{environment}}` will call back with `environment`
- An input text of `{{rural}} please` will call back with `rural`

Each token detected will result in a separate call back to DMR. For example, input text of `I want to see {{rural}}{{social}} and {{environment}}` will return `rural`, `social` and `environment` as three separate DMR call backs.

We want the Classifier to support random ministry selection to help with testing, therefore is the `{{random}}` token is used, the Classifier will randomly select and return one of the 11 ministries. For example, an input text of `Give me a {{random}} one` will return a randomly selected ministry.

Spaces are not permitted so an input text of `Please return {{ environment}}` would be treated as an error case.

If a token is detected with unrecognised text (i.e. text that does not exactly map to one of the labels defined in the Ministries section or the word `random`), this will be treated as an error case. For example, the input text of `Please return {{educationandresearch}}` would be an error as would `{{Please return {{education,social}}`.

## API Design

The Classifier will be a REST api with just a single endpoint which is `/input-from-dmr/institution`.

#### Request

Accepts only `POST` requests. Other http verbs will result in `404/NotFound` responses.

Expects an `message` parameter which is an array of strings. The strings contains the text representing the input text. This will be defined in a JSON request body, for example

```json
{
"message":["I want to register my child at school"]
}
```

or

```json
{
"message":["Please return the {{social}} minsitry"]
}
```

Other parameters may also be required in order for the Classifier to call back to DMR when the processing is completed (call-back uri for example). Please consult the DMR design documentation for details of these parameters.

#### Response

For the initial response, the Classifier will return `202/Accepted` with an empty body.

The appropriate http status codes will be returned for error scenarios.

#### Call-back

The API will then do it's work to establish the ministry.

When the work is complete, the Classifier will call the DMR API with a POST request.

The request body will be json format following this schema (the value can be any of the label outlined in the Ministries section).

```json
{
"ministry":"education"
}
```

## Hosting

The Classifier will be deployed and hosted as a container on a Kubernetes cluster and will follow the foundation design for Kubernetes in terms of configuration.
