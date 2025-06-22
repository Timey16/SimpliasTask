# SimpliasTask
Project for the interview process with Simplias GmbH

# Launch DeepSeek locally
first make sure that ollama is installed, instructions are found here:
https://www.centron.de/en/tutorial/ollama-installation-guide-run-llms-locally-on-linux-windows-macos/
check if ollama has been correctly installed by typing ```ollama --version``` in the command line.

Afterwards download DeepSeek's small model for local use via the command line by using the command
```ollama pull deepseek-r1:1.5b```
then provide it locally via
```ollama serve```

# Installing the Backend
If the nuget CLI is installed navigate to the ```backend``` folder and type ```nuget install``` to install the required dependencies or use the Solution with Visual Studio to do so.

# Installing the Frontend
Navigate to the ```frontend``` folder using the CLI console and type ```npm install``` to install the required packages for the frontend.

# Launching the project
1. make sure DeepSeek is running locally
2. open the Solution file (```SimpliasTask.sln``` in the root folder)
3. press "start" to launch the application. Both backend and frontend will both launch simultaneously. You may have to accept temporary certificates.
4. Two browser windows will open. One is the frontend and one will be swagger that represents the API to send commands more directly.

# Reasonings and explanations
For the database I chose to use SQLite since the small size of the project means it can run alongside the server on the same system, an entirely own SQL Server won't be required.
The account system doesn't currently distinguish in tasks based on which user created said task as time didn't permit this.

Furthermore I chose DeepSeek is the AI in question as it is able to be deployed locally, and with a lightweight model, that doesn't require the use of an account or any tokens, making it ideal for demo projects such as this one.
The Backend doesn't await the answer of the AI while creating a new task, it will receive the answer and push it with SignalR once it has become available.
I had to reverse engineer the .NET library for the ChatResponse and create my own one as it turned out the model of the response by DeepSeek has changed and it no longer corresponded to what the library was providing. The library assumed the answer would be in a "Choices" array, however now the answer is within a "Message" object.
The prompt the Backend sends to DeepSeek is:
```
Create a suggested priority for the following title and description for a given task. Answer in only a single word.
Allowed answers are only "low", "medium" and "high" for the priority of said task.
The title of the tasks is: "{ title }"
The description of the task is: "{ description}
```

For the frontend I chose Angular and NG-Zorro as it provides a quick framework with a lot of functioning components and icons to use making it also ideal for demo projects such as this one.

First I implemented the controllers and the login system as well as the databse. Swagger allows me to send test queries to my API to check out what it reports back. Once that was finished I created the frontend. First the login and signup, then the tasklist. Finally I implemented the AI operation.

# Possible features if that project would be maintained in the future
1. adding user roles
2. being able to create tasks for users and for roles
3. as DeepSeek adds the reasoning for the priority, add a modal that's being opened when the priority is clicked that shows the reasoning DeepSeek gave
4. allow users to add and configure their own AI APIs (so being able to connect OpenAI, xAI, GeminiAI, etc.), even if it will remove the reasoning.
5. allow users to have the AI determine priority of tasks by taking all previous, currently active, tasks into account and maybe even re-evaluate the priority of these tasks. Something that may be urgent on it's own may not be so urgent relative to other problems.
6. multi language support.
7. Combined application so it can be used by people complete on their own machine without the need to start anything so it can be distributed as a standalone piece of organization software for home-users.
8. Sorting and filtering on the list.
9. Start Dates, End Dates, duration, repetition, categories and tags for tasks (and them being accounted for by the AI).