import React, { useState } from 'react';
import TaskList from './components/TaskList';
import Login from './components/Login';
import './App.css'; 

const App: React.FC = () => {
  const [token, setToken] = useState(localStorage.getItem('jwtToken') || '');

  const handleLogin = (jwt: string) => {
    localStorage.setItem('jwtToken', jwt);
    setToken(jwt);
  };

  const handleLogout = () => {
    localStorage.removeItem('jwtToken');
    setToken('');
  };

  return (
      <div className="App">   
          {!token ? (
              <Login onLogin={handleLogin} />
          ) : (
              <>
<button onClick={handleLogout}>Logout</button>
<TaskList />
</>
          )}
    </div>
  );
};

export default App;
