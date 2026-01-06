import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { authService } from './services/api';
import Login from './components/Login';
import Dashboard from './components/Dashboard';
import Courses from './components/Courses';
import Lessons from './components/Lessons';
import './App.css';

function PrivateRoute({ children }) {
  return authService.isAuthenticated() ? children : <Navigate to="/" />;
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route
          path="/dashboard"
          element={
            <PrivateRoute>
              <Dashboard />
            </PrivateRoute>
          }
        />
        <Route
          path="/courses"
          element={
            <PrivateRoute>
              <Courses />
            </PrivateRoute>
          }
        />
        <Route
          path="/courses/:courseId/lessons"
          element={
            <PrivateRoute>
              <Lessons />
            </PrivateRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
