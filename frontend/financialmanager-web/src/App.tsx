import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AppLayout } from "./components/Layout/AppLayout";
import DashboardPage from "./pages/Dashboard/DashboardPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppLayout />}>
          <Route path="/" element={<DashboardPage />} />

          {/* placeholders */}
          <Route path="/pessoas" element={<div style={{ color: "#fff" }}>Pessoas (em breve)</div>} />
          <Route path="/categorias" element={<div style={{ color: "#fff" }}>Categorias (em breve)</div>} />
          <Route path="/transacoes" element={<div style={{ color: "#fff" }}>Transações (em breve)</div>} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
