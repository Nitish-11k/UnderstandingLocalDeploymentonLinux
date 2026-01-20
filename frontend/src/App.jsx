import { useState } from 'react';

function App() {
  const [users, setUsers] = useState([]);
  const [status, setStatus] = useState("Click to check Health");

  // Function to check API Health
  const checkHealth = async () => {
    try {
      const res = await fetch('/api/health');
      if (res.ok) {
        const data = await res.json();
        setStatus(`Backend is: ${data.status} at ${data.timestamp}`);
      } else {
        setStatus("Backend Error");
      }
    } catch (err) {
      setStatus("Connection Failed (Is Nginx Proxy setup?)");
    }
  };

  // Function to get Users
  const getUsers = async () => {
    try {
      const res = await fetch('/api/users');
      const data = await res.json();
      setUsers(data);
    } catch (err) {
      console.error("Failed to fetch users", err);
    }
  };

  return (
    <div style={{ padding: "20px", fontFamily: "sans-serif" }}>
      <h1>Deployment Practice Dashboard</h1>

      <div style={{ border: "1px solid #ccc", padding: "10px", marginBottom: "20px" }}>
        <h2>System Health</h2>
        <button onClick={checkHealth}>Check API Status</button>
        <p><strong>Status:</strong> {status}</p>
      </div>

      <div style={{ border: "1px solid #ccc", padding: "10px" }}>
        <h2>User Database</h2>
        <button onClick={getUsers}>Load Users</button>
        <ul>
          {users.map((u, i) => <li key={i}>{u}</li>)}
        </ul>
      </div>
    </div>
  );
}

export default App;