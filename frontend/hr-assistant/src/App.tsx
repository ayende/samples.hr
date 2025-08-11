import React from 'react';
import styled, { createGlobalStyle } from 'styled-components';
import { ChatInterface } from './components/ChatInterface';
import { theme } from './theme';

const GlobalStyle = createGlobalStyle`
  * {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
  }

  body {
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
      'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
      sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    background: ${theme.colors.background.primary};
    color: ${theme.colors.text.primary};
  }

  code {
    font-family: source-code-pro, Menlo, Monaco, Consolas, 'Courier New',
      monospace;
  }

  html, body, #root {
    height: 100%;
  }
`;

const AppContainer = styled.div`
  height: 100vh;
  display: flex;
  flex-direction: column;
`;

function App() {
  return (
    <>
      <GlobalStyle />
      <AppContainer>
        <ChatInterface />
      </AppContainer>
    </>
  );
}

export default App;
