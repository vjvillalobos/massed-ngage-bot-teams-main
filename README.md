# MassEdNgage
Bot para Teams que participa de las reuniones y envía cambios de estado a Ngage ETL
C# y .NET, fork de 
MassEd + Practia = Ngage

Petición de prueba:
POST
https://botpractiaincidentbot.azurewebsites.net/classes/join
{
    "name": "Clase ejemplo 001",
    "classId": "123456",
    "time": "2020-12-01T13:00:00.0000Z",
    "tenantId": "6f961525-8fda-4100-b239-72e39db81e85",
    "joinURL": "https://teams.microsoft.com/l/meetup-join/19%3a2eb7376b2a914fe0b21524aac738a0ea%40thread.tacv2/1606839624774?context=%7b%22Tid%22%3a%226f961525-8fda-4100-b239-72e39db81e85%22%2c%22Oid%22%3a%22f4033699-4ce5-453b-a996-d1283d13abd0%22%7d",
    "removeFromDefaultRoutingGroup": true,
    "allowConversationWithoutHost": true
}

Respuesta:
{
  "status": "OK",
  "callURI": "https://teams.microsoft.com/l/meetup-join/19%3a2eb7376b2a914fe0b21524aac738a0ea%40thread.tacv2/1606839624774?context=%7b%22Tid%22%3a%226f961525-8fda-4100-b239-72e39db81e85%22%2c%22Oid%22%3a%22f4033699-4ce5-453b-a996-d1283d13abd0%22%7d",
  "classID": "123456"
}