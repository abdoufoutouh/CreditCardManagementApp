<template>
  <DashboardLayout title="Dashboard" username="John Doe">
    <div class="dashboard-content">
      <WelcomeSection />
      
      <StatsSection :stats="stats" />
      
      <CardsSection 
        :cards="creditCards" 
        :loading="isLoadingCards"
        @show-details="handleShowDetails"
        @update="handleUpdate"
        @delete="handleDelete"
      />
      
      <ActivitySection />
    </div>
  </DashboardLayout>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import DashboardLayout from '../components/layout/DashboardLayout.vue';
import WelcomeSection from '../components/sections/WelcomeSection.vue';
import StatsSection from '../components/sections/StatsSection.vue';
import CardsSection from '../components/sections/CardsSection.vue';
import ActivitySection from '../components/sections/ActivitySection.vue';
import { useCreditCardAPI } from '../composables/useCreditCardAPI';

const { getCreditCards, isLoading: isLoadingCards } = useCreditCardAPI();

const rawCreditCards = ref([]);
const errorMessage = ref('');

// Transform backend data to match component expectations
const creditCards = computed(() => {
  return rawCreditCards.value.map(card => {
    // Si le backend ne retourne pas le type, on essaie de le détecter
    let cardType = card.type;
    if (!cardType && card.cardNumberPartial) {
      // Essayer de détecter à partir du numéro partiel (pas fiable mais mieux que rien)
      if (card.cardNumberPartial.startsWith('4')) {
        cardType = 'Visa';
      } else if (card.cardNumberPartial.startsWith('5')) {
        cardType = 'Mastercard';
      } else if (card.cardNumberPartial.startsWith('3')) {
        cardType = 'Amex';
      }
    }
    
    return {
      id: card.id,
      cardType: cardType || 'VISA',
      cardNumber: card.cardNumberPartial || '',
      cardholderName: 'CARD HOLDER',
      expirationDate: formatExpirationDate(card.expirationDate),
      creditLimit: card.creditLimit || 0,
      currentBalance: card.currentBalance || 0,
      isActive: card.isActive || false
    };
  });
});

// Calculate stats based on real data
const stats = computed(() => {
  const totalCards = rawCreditCards.value.length;
  const activeCards = rawCreditCards.value.filter(card => card.isActive).length;
  const totalBalance = rawCreditCards.value.reduce((sum, card) => sum + (card.currentBalance || 0), 0);
  
  return [
    {
      id: 1,
      title: 'Total Cards',
      value: totalCards.toString(),
      icon: '💳',
      iconColor: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
    },
    {
      id: 2,
      title: 'Active Cards',
      value: activeCards.toString(),
      icon: '✅',
      iconColor: 'linear-gradient(135deg, #4CAF50 0%, #45a049 100%)'
    },
    {
      id: 3,
      title: 'Total Balance',
      value: `$${totalBalance.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`,
      icon: '💰',
      iconColor: 'linear-gradient(135deg, #FF6B6B 0%, #FF8E53 100%)'
    },
    {
      id: 4,
      title: 'Last Activity',
      value: '2h ago', // This would need to come from backend
      icon: '🕐',
      iconColor: 'linear-gradient(135deg, #4ECDC4 0%, #44A08D 100%)'
    }
  ];
});

const formatExpirationDate = (dateString) => {
  if (!dateString) return 'MM/YY';
  
  try {
    const date = new Date(dateString);
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = String(date.getFullYear()).slice(-2);
    return `${month}/${year}`;
  } catch (error) {
    return 'MM/YY';
  }
};

const loadCreditCards = async () => {
  try {
    const result = await getCreditCards();
    
    if (result.success) {
      rawCreditCards.value = result.data || [];
    } else {
      errorMessage.value = result.error || 'Failed to load credit cards';
      console.error('Failed to load credit cards:', result.error);
    }
  } catch (error) {
    errorMessage.value = error.message || 'An unexpected error occurred';
    console.error('Error loading credit cards:', error);
  }
};

const handleShowDetails = (cardId) => {
  console.log('Show details for card:', cardId);
  // TODO: Implement show details functionality
};

const handleUpdate = (cardId) => {
  console.log('Update card:', cardId);
  // TODO: Implement update functionality
};

const handleDelete = async (cardId) => {
  console.log('Delete card:', cardId);
  // TODO: Implement delete functionality
  // After successful delete, reload cards
  // await loadCreditCards();
};

// Load cards when component mounts
onMounted(() => {
  loadCreditCards();
});
</script>

<style scoped>
.dashboard-content {
  display: flex;
  flex-direction: column;
  gap: 30px;
}
</style>
