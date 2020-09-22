# DurableFunctionsSagaTest

Proof of concept code around an import/export orchestration that uses Azure Durable Entities to store state

We model an invoice export from CRM to Fiannce transaction using a Durable Orchestration and Activity functions for talking to each system.

Export of an invoice is triggered by an Http trigger.

We persist ongoing transaction state ina Durable Entity. 

Another HTTP function provides an endpoint to retrieve the state of all transactions in the system, this could feed int oa dashboard website.

