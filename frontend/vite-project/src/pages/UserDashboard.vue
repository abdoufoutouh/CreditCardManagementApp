<template>
  <DashboardLayout title="Dashboard" username="John Doe">
    <div class="dashboard-content">
      <WelcomeSection />
      
      <StatsSection :stats="stats" />

      <SearchCardsSection @search-results="handleSearchResults" />
      
      <CardsSection 
        :cards="displayedCards" 
        :loading="isLoadingCards"
        :cardholder-name="cardholderName"
        @show-details="handleShowDetails"
        @update="handleUpdate"
        @delete="handleDelete"
      />
    </div>
  </DashboardLayout>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import DashboardLayout from '../components/layout/DashboardLayout.vue';
import WelcomeSection from '../components/sections/WelcomeSection.vue';
import StatsSection from '../components/sections/StatsSection.vue';
import SearchCardsSection from '../components/sections/SearchCardsSection.vue';
import CardsSection from '../components/sections/CardsSection.vue';
import { useCreditCardAPI } from '../composables/useCreditCardAPI';
import { useAuthStore } from '../store/authStore';

const router = useRouter();
const authStore = useAuthStore();
const { getCreditCards, deleteCreditCard, isLoading: isLoadingCards } = useCreditCardAPI();

const rawCreditCards = ref([]);
const searchResults = ref(null);
const errorMessage = ref('');

// Get cardholder name from authenticated user
const cardholderName = computed(() => {
  if (authStore.user?.firstName && authStore.user?.lastName) {
    return `${authStore.user.firstName} ${authStore.user.lastName}`.toUpperCase();
  }
  return 'CARD HOLDER';
});

// Transform backend data to match component expectations
const transformCards = (cards) => {
  return cards.map(card => {
    let cardType = card.type;
    if (!cardType && card.cardNumberPartial) {
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
      cardholderName: cardholderName.value,
      expirationDate: formatExpirationDate(card.expirationDate),
      creditLimit: card.creditLimit || 0,
      currentBalance: card.currentBalance || 0,
      isActive: card.isActive || false
    };
  });
};

// Display either search results or all cards
const displayedCards = computed(() => {
  if (searchResults.value !== null) {
    return transformCards(searchResults.value);
  }
  return transformCards(rawCreditCards.value);
});

// Calculate stats based on real data
const stats = computed(() => {
  const totalCards = rawCreditCards.value.length;
  const totalBalance = rawCreditCards.value.reduce((sum, card) => sum + (card.currentBalance || 0), 0);
  
  // Count cards expiring within 90 days
  const now = new Date();
  const ninetyDaysFromNow = new Date(now.getTime() + 90 * 24 * 60 * 60 * 1000);
  const expiringCards = rawCreditCards.value.filter(card => {
    if (!card.expirationDate) return false;
    const expDate = new Date(card.expirationDate);
    return expDate <= ninetyDaysFromNow && expDate > now;
  }).length;
  
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
      title: 'Expiring Soon',
      value: expiringCards.toString(),
      icon: '⏰',
      iconColor: 'linear-gradient(135deg, #FF6B6B 0%, #FF8E53 100%)'
    },
    {
      id: 3,
      title: 'Total Balance',
      value: `${totalBalance.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`,
      icon: '💰',
      iconColor: 'linear-gradient(135deg, #4CAF50 0%, #45a049 100%)'
    },
    {
      id: 4,
      title: 'Last Activity',
      value: '2h ago',
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

const handleSearchResults = (results) => {
  searchResults.value = results.length > 0 ? results : null;
};

const handleShowDetails = (cardId) => {
  console.log('Show details for card:', cardId);
};

const handleUpdate = (cardId) => {
  router.push(`/creditcards/edit/${cardId}`);
};

const handleDelete = async (cardId) => {
  if (!confirm('Are you sure you want to delete this credit card?')) {
    return;
  }

  try {
    const result = await deleteCreditCard(cardId);
    
    if (result.success) {
      errorMessage.value = '';
      await loadCreditCards();
      searchResults.value = null;
      alert('Credit card deleted successfully!');
    } else {
      errorMessage.value = result.error || 'Failed to delete credit card';
      alert(`Error: ${result.error || 'Failed to delete credit card. Make sure the card has a zero balance.'}`);
      console.error('Failed to delete credit card:', result);
    }
  } catch (error) {
    errorMessage.value = error.message || 'An unexpected error occurred';
    alert(`Error: ${error.message || 'An unexpected error occurred'}`);
    console.error('Error deleting credit card:', error);
  }
};

onMounted(() => {
  authStore.initializeAuth();
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
