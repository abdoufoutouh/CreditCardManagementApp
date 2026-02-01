<template>
  <div class="activity-section">
    <div class="section-header">
      <h2 class="section-title">Activity Overview</h2>
    </div>
    <div class="chart-container">
      <div class="chart-placeholder">
        <div class="chart-bars">
          <div class="bar" style="height: 60%;"></div>
          <div class="bar" style="height: 80%;"></div>
          <div class="bar" style="height: 45%;"></div>
          <div class="bar" style="height: 90%;"></div>
          <div class="bar" style="height: 70%;"></div>
          <div class="bar" style="height: 85%;"></div>
          <div class="bar" style="height: 55%;"></div>
          <div class="bar" style="height: 75%;"></div>
          <div class="bar" style="height: 65%;"></div>
          <div class="bar" style="height: 95%;"></div>
          <div class="bar" style="height: 50%;"></div>
          <div class="bar" style="height: 80%;"></div>
        </div>
        <div class="chart-labels">
          <span>Jan</span>
          <span>Feb</span>
          <span>Mar</span>
          <span>Apr</span>
          <span>May</span>
          <span>Jun</span>
          <span>Jul</span>
          <span>Aug</span>
          <span>Sep</span>
          <span>Oct</span>
          <span>Nov</span>
          <span>Dec</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.activity-section {
  background: white;
  border-radius: 16px;
  padding: 30px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  border: 1px solid #f0f0f0;
}

.section-header {
  margin-bottom: 25px;
}

.section-title {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: #2c3e50;
}

.chart-container {
  height: 300px;
  position: relative;
}

.chart-placeholder {
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
}

.chart-bars {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  height: 250px;
  padding: 0 10px;
  gap: 8px;
}

.bar {
  flex: 1;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 4px 4px 0 0;
  min-height: 20px;
  transition: all 0.3s ease;
  cursor: pointer;
}

.bar:hover {
  opacity: 0.8;
  transform: scaleY(1.05);
}

.chart-labels {
  display: flex;
  justify-content: space-between;
  padding: 15px 10px 0 10px;
  font-size: 12px;
  color: #7f8c8d;
  font-weight: 500;
}

@media (max-width: 768px) {
  .chart-labels {
    font-size: 10px;
  }
  
  .chart-bars {
    gap: 4px;
  }
}
</style>
