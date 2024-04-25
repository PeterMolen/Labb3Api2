# Labb3Api2
Swagger anrop och Json svar ifrån uppgiten:
  _____________________________________________

Hämta alla personer i systemet

```json
[
  {
    "personId": 1,
    "name": "Pete",
    "phone": "1337"
  },
  {
    "personId": 3,
    "name": "Petrus",
    "phone": "Maximus@gmail.com"
  }
]


  Hämta alla intressen som är kopplade till en specifik person
  {
  "person": {
    "personId": 1,
    "name": "Pete",
    "phone": "1337"
  },
  "interests": [
    {
      "interestId": 1,
      "title": "Basket",
      "description": "A sport"
    }


    Hämta alla länkar som är kopplade till en specifik person
    {
    "person": {
      "personId": 3,
      "name": "Petrus",
      "phone": "Maximus@gmail.com"
    },
    "links": [
      {
        "linkId": 1,
        "url": "www.NHL.com",
        "interest": {
          "interestId": 4,
          "title": "Hockey",
          "description": "A puck n stick"
        }
      },
      {
        "linkId": 2,
        "url": "www.premierleague.com",
        "interest": {
          "interestId": 5,
          "title": "Fotball",
          "description": "The boll is round"
        }
      }
    ]
  }


Koppla en person till ett nytt intresse
för ett intresse:
 {
  "fkPersonId": 1,
  "interestIds": [1]
}

flera intressen:
{
  "fkPersonId": 0,
  "interestIds": [1,2,3]
}


Lägga in nya länkar för en specifik person och ett specifikt intresse
{
  "fkPersonId": 1,
  "links": [
    {
      "fkInterestId": 1,
      "url": "www.nba.com"
    }
  ]
}


flera intrsssen/länkar:
{
  "fkPersonId": 1,
  "links": [
    {
      "fkInterestId": 1,
      "url": "www.example1.com"
    },
    {
      "fkInterestId": 2,
      "url": "www.example2.com"
    }
  ]
}

    
  
