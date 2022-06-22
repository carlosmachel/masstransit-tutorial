import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { useEffect, useRef, useState } from "react";
import "./App.css";

function App() {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const [notifications, setNotifications] = useState<string[]>([]);

  const ref = useRef<string[]>([]);

  ref.current = notifications;

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl("https://localhost:7254/orderNotification")
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  // const orderAcceptedHandle = (message: any) => {
  //   setNotifications([...notifications, JSON.stringify(message)]);
  // };

  useEffect(() => {
    async function connect() {
      try {
        if (connection) {
          connection.start();
          connection.on("OrderAccepted", (message: any) => {
            const updatedChat = [...ref.current];
            updatedChat.push(JSON.stringify(message));

            setNotifications(updatedChat);
          });
        }
      } catch (error) {
        console.log("Connection failed: ", error);
      }
    }

    connect();
  }, [connection]);

  return (
    <div className="App">
      <header className="App-header">
        {notifications.map((notification, index) => (
          <p key={index}>{notification}</p>
        ))}
      </header>
    </div>
  );
}

export default App;
