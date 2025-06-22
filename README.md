# SimpliasTask
Project for the interview process with Simplias GmbH

# Launch DeepSeek locally

first make sure that ollama is installed, instructions are found here:
https://www.centron.de/en/tutorial/ollama-installation-guide-run-llms-locally-on-linux-windows-macos/
check if ollama has been correctly installed by typing ```ollama --version``` in the command line.

Afterwards download DeepSeek's small model for local use via the command line.
```ollama pull deepseek-r1:1.5b```
then provide it locally via
```ollama serve```