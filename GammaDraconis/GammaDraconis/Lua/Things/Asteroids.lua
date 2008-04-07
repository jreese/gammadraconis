--[[ 
  Asteroid Definitions 
  ----
  Asteroid800A
  ]]--
  
-- AsteroidBase
base = GameObject()
base.dragR = 0
base.dragL = 0
base.mass = 100000
base.size = 75
base.maxHealth = 100

function asteroidDead(asteroid)
	Engine.GetInstance().gameScene:ignore(asteroid, GO_TYPE.DEBRIS)
	if asteroid.maxHealth > base.maxHealth / 8 then
		local roid1 = asteroid:clone()
		roid1.size = asteroid.size / 2
		roid1:scaleModels(0.5)
		roid1.mass = asteroid.mass / 2
		roid1.maxHealth = asteroid.maxHealth / 2
		roid1.velocity = asteroid.velocity;
		local oldPos = asteroid.position:pos()
		local roid2 = roid1:clone()
		local roid3 = roid1:clone()
		local roid4 = roid1:clone()
		local roid5 = roid1:clone()
		local roid6 = roid1:clone()
		local roid7 = roid1:clone()
		local roid8 = roid1:clone()
		local offset = roid1.size * 0.99
		roid1.position = Coords(oldPos.X + offset, oldPos.Y + offset, oldPos.Z + offset)
		roid2.position = Coords(oldPos.X + offset, oldPos.Y + offset, oldPos.Z - offset)
		roid3.position = Coords(oldPos.X + offset, oldPos.Y - offset, oldPos.Z + offset)
		roid4.position = Coords(oldPos.X + offset, oldPos.Y - offset, oldPos.Z - offset)
		roid5.position = Coords(oldPos.X - offset, oldPos.Y + offset, oldPos.Z + offset)
		roid6.position = Coords(oldPos.X - offset, oldPos.Y + offset, oldPos.Z - offset)
		roid7.position = Coords(oldPos.X - offset, oldPos.Y - offset, oldPos.Z + offset)
		roid8.position = Coords(oldPos.X - offset, oldPos.Y - offset, oldPos.Z - offset)
		Engine.GetInstance().gameScene:track(roid1, GO_TYPE.DEBRIS)
		Engine.GetInstance().gameScene:track(roid2, GO_TYPE.DEBRIS)
		Engine.GetInstance().gameScene:track(roid3, GO_TYPE.DEBRIS)
		Engine.GetInstance().gameScene:track(roid4, GO_TYPE.DEBRIS)
		Engine.GetInstance().gameScene:track(roid5, GO_TYPE.DEBRIS)
		Engine.GetInstance().gameScene:track(roid6, GO_TYPE.DEBRIS)
		Engine.GetInstance().gameScene:track(roid7, GO_TYPE.DEBRIS)
		Engine.GetInstance().gameScene:track(roid8, GO_TYPE.DEBRIS)
	end
end

base.OnDeathFunction = GammaDraconis.GameLua:GetFunction("asteroidDead")

-- Asteroid800A
fbx = FBXModel("Resources/Models/Asteroid800A", "", 200)

roid = base:clone()
roid.models:Add(fbx)

Proto.thing:Add("Asteroid800A", roid)

-- Asteroid800B
fbx = FBXModel("Resources/Models/Asteroid800B", "", 200)

roid = base:clone()
roid.models:Add(fbx)

Proto.thing:Add("Asteroid800B", roid)
