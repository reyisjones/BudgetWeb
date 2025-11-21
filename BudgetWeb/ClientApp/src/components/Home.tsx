import { connect } from 'react-redux';

const Home = () => (
  <div>
    <h1>Budget App</h1>
    <p>Welcome to Budget App, a modern cross-platform desktop application built with:</p>
    <ul>
      <li><a href='https://dotnet.microsoft.com/'>ASP.NET Core (.NET 10)</a> and <a href='https://learn.microsoft.com/en-us/dotnet/csharp/'>C# 13</a> for high-performance server-side code</li>
      <li><a href='https://react.dev/'>React 18</a> and <a href='https://redux-toolkit.js.org/'>Redux Toolkit</a> for modern client-side state management</li>
      <li><a href='https://getbootstrap.com/'>Bootstrap 5</a> for responsive layout and styling</li>
      <li><a href='https://vitejs.dev/'>Vite</a> for lightning-fast development with HMR</li>
      <li><a href='https://github.com/ElectronNET/Electron.NET'>Electron.NET</a> for cross-platform desktop support (macOS, Windows, Linux)</li>
    </ul>
    <p>This application can run as both a web app and a native desktop application, featuring:</p>
    <ul>
      <li><strong>Cross-platform desktop</strong>. Runs natively on macOS, Windows, and Linux using Electron.NET.</li>
      <li><strong>Modern React development</strong>. Built with React 18, React Router v6, and Vite for instant HMR.</li>
      <li><strong>.NET 10 backend</strong>. Powered by the latest .NET runtime with minimal APIs and 30-40% performance improvements over .NET 6.</li>
      <li><strong>Efficient production builds</strong>. Vite produces optimized, tree-shaken JavaScript bundles with code splitting.</li>
      <li><strong>Type safety</strong>. TypeScript 5.6 throughout the frontend with strict mode enabled.</li>
    </ul>
    <p>The <code>ClientApp</code> subdirectory is a modern React application using Vite. Run <code>npm start</code> for development or <code>npm run build</code> for production.</p>
    <p>To run as a desktop app: <code>electronize start</code></p>
  </div>
);

export default connect()(Home);

